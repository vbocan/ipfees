using IPFLang.Evaluator;
using IPFLang.Parser;

namespace IPFLang.Engine
{
    public class DslCalculator : IDslCalculator
    {
        private IDslParser Parser { get; set; }

        public DslCalculator(IDslParser parser)
        {
            this.Parser = parser;
        }

        public bool Parse(string text)
        {
            return Parser.Parse(text);
        }

        public IEnumerable<string> GetErrors() => Parser.GetErrors().Select(s => s.Item2);
        public IEnumerable<DslInput> GetInputs() => Parser.GetInputs();
        public IEnumerable<DslFee> GetFees() => Parser.GetFees();
        public IEnumerable<DslReturn> GetReturns() => Parser.GetReturns();
        public IEnumerable<DslGroup> GetGroups() => Parser.GetGroups();

        public (decimal, decimal, IEnumerable<string>, IEnumerable<(string, string)>) Compute(IEnumerable<IPFValue> InputValues)
        {
            decimal TotalMandatoryAmount = 0;
            decimal TotalOptionalAmount = 0;
            var ComputeSteps = new List<string>();

            foreach (var fee in Parser.GetFees())
            {
                if (fee.Optional)
                {
                    ComputeSteps.Add(string.Format("COMPUTING OPTIONAL FEE [{0}]", fee.Name));
                }
                else
                {
                    ComputeSteps.Add(string.Format("COMPUTING FEE [{0}]", fee.Name));
                }
                // Compose fee local variables
                var AllVars = new List<IPFValue>();
                AllVars.AddRange(InputValues);
                foreach (var fv in fee.Vars)
                {
                    var fv_val = DslEvaluator.EvaluateExpression(fv.ValueTokens.ToArray(), AllVars, fee.Name);
                    var fee_val = new IPFValueNumber(fv.Name, fv_val);
                    AllVars.Add(fee_val);
                }
                // Proceed with computation
                decimal CurrentAmount = 0;
                ComputeSteps.Add(string.Format("Amount is initially {0}", CurrentAmount));
                foreach (DslFeeCase fc in fee.Cases.Cast<DslFeeCase>())
                {
                    var case_cond = DslEvaluator.EvaluateLogic(fc.Condition.ToArray(), AllVars, fee.Name);
                    if (!case_cond)
                    {
                        ComputeSteps.Add(string.Format("Condition [{0}] is FALSE, skipping", string.Join(" ", fc.Condition)));
                        continue;
                    }
                    if (fc.Condition.Any()) ComputeSteps.Add(string.Format("Condition [{0}] is TRUE, proceeding with evaluating individual expressions", string.Join(" ", fc.Condition)));
                    foreach (var b in fc.Yields)
                    {
                        var cond_b = DslEvaluator.EvaluateLogic(b.Condition.ToArray(), AllVars, fee.Name);
                        var val_b = DslEvaluator.EvaluateExpression(b.Values.ToArray(), AllVars, fee.Name);
                        if (b.Condition.Any()) ComputeSteps.Add(string.Format("Condition: [{0}] is [{1}]", string.Join(" ", b.Condition), cond_b));
                        if (cond_b)
                        {
                            CurrentAmount += val_b;
                            ComputeSteps.Add(string.Format("Expression [{0}] evaluates to [{1}], therefore the new amount is {2}", string.Join(" ", b.Values), val_b, CurrentAmount.ToString("0.00")));
                        }
                    }
                }
                ComputeSteps.Add(string.Format("The final amount for fee {0} is {1}", fee.Name, CurrentAmount.ToString("0.00")));
                ComputeSteps.Add(string.Empty);
                if (fee.Optional)
                {
                    TotalOptionalAmount += CurrentAmount;
                }
                else
                {
                    TotalMandatoryAmount += CurrentAmount;
                }

            }
            ComputeSteps.Add(string.Format("Total amount for mandatory fees: [{0}]", TotalMandatoryAmount.ToString("0.00")));
            ComputeSteps.Add(string.Format("Total amount for optional fees: [{0}]", TotalOptionalAmount.ToString("0.00")));
            ComputeSteps.Add(string.Format("Grand total: [{0}]", (TotalMandatoryAmount + TotalOptionalAmount).ToString("0.00")));
            var Returns = Parser.GetReturns().Select(s => (s.Symbol, s.Text));
            return (TotalMandatoryAmount, TotalOptionalAmount, ComputeSteps, Returns);
        }

        public void Reset()
        {
            Parser.Reset();
        }
    }
}
