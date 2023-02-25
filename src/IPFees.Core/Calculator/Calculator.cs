using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Calculator
{
    public class IPFCalculator : IIPFCalculator
    {
        private IPFParser parser;

        public IPFCalculator() { }

        public bool Parse(string text)
        {
            parser = new IPFParser(text);
            return parser.Parse();
        }

        public IEnumerable<string> GetErrors() => parser.GetErrors().Select(s => s.Item2);
        public IEnumerable<IPFVariable> GetVariables() => parser.GetVariables();
        public IEnumerable<IPFFee> GetFees() => parser.GetFees();

        public (double, double, IEnumerable<string>) Compute(IPFValue[] vars)
        {
            double TotalMandatoryAmount = 0;
            double TotalOptionalAmount = 0;
            var ComputeSteps = new List<string>();

            foreach (var fee in parser.GetFees())
            {
                if (fee.Optional)
                {
                    ComputeSteps.Add(string.Format("COMPUTING OPTIONAL FEE [{0}]", fee.Name));
                }
                else
                {
                    ComputeSteps.Add(string.Format("COMPUTING FEE [{0}]", fee.Name));
                }
                double CurrentAmount = 0;
                ComputeSteps.Add(string.Format("Amount is initially {0}", CurrentAmount));
                foreach (IPFFeeCase fc in fee.Cases.Cast<IPFFeeCase>())
                {
                    var case_cond = (!fc.Condition.Any()) || IPFEvaluator.EvaluateLogic(fc.Condition.ToArray(), vars);
                    if (!case_cond)
                    {
                        ComputeSteps.Add(string.Format("Condition [{0}] is FALSE, skipping", string.Join(" ", fc.Condition)));
                        continue;
                    }
                    if (fc.Condition.Any()) ComputeSteps.Add(string.Format("Condition [{0}] is TRUE, proceeding with evaluating individual expressions", string.Join(" ", fc.Condition)));
                    foreach (var b in fc.Yields)
                    {
                        var cond_b = IPFEvaluator.EvaluateLogic(b.Condition.ToArray(), vars);
                        var val_b = IPFEvaluator.EvaluateExpression(b.Values.ToArray(), vars);
                        if (b.Condition.Any()) ComputeSteps.Add(string.Format("Condition: [{0}] is [{1}]", string.Join(" ", b.Condition), cond_b));
                        if (cond_b)
                        {
                            CurrentAmount += val_b;
                            ComputeSteps.Add(string.Format("Expression [{0}] evaluates to [{1}], so the new amount is {2}", string.Join(" ", b.Values), val_b, CurrentAmount.ToString("0.00")));
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
            return (TotalMandatoryAmount, TotalOptionalAmount, ComputeSteps);
        }
    }
}