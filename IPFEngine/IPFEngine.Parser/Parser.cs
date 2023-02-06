using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFEngine.Parser
{
    public class IPFParser
    {
        string[] IPFData { get; set; }
        Parsing CurrentlyParsing = Parsing.None;

        private IPFVariableList CurrentList { get; set; } = new IPFVariableList(string.Empty, string.Empty, new List<IPFListItem>(), string.Empty);
        private IPFVariableNumber CurrentNumber { get; set; } = new IPFVariableNumber(string.Empty, string.Empty, int.MinValue, int.MaxValue, 0);
        private IPFVariableBoolean CurrentBoolean { get; set; } = new IPFVariableBoolean(string.Empty, string.Empty, false);
        private IPFFee CurrentFee { get; set; } = new IPFFee(string.Empty, new List<IPFItem>());
        private IPFFeeCase CurrentFeeCase { get; set; } = new IPFFeeCase(Enumerable.Empty<string>(), new List<IPFFeeYield>());

        private IList<IPFVariable> Variables { get; set; } = new List<IPFVariable>();
        private IList<IPFFee> Fees { get; set; } = new List<IPFFee>();


        public IPFParser(string source)
        {
            IPFData = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public (IEnumerable<IPFVariable>, IEnumerable<IPFFee>) Parse()
        {
            Func<string[], bool>[] IPFParsers = new Func<string[], bool>[]
            {
                ParseList,
                ParseListValue,
                ParseListDefaultValue,
                ParseNumber,
                ParseNumberBetween,
                ParseNumberDefault,
                ParseBoolean,
                ParseBooleanDefault,
                ParseEndDefine,
                ParseComment,
                ParseFee,
                ParseFeeCase,
                ParseFeeYield,
                ParseFeeEndCase,
                ParseEndCompute
            };

            for (int i = 0; i < IPFData.Length; i++)
            {
                string line = IPFData[i].Trim();
                if (string.IsNullOrEmpty(line)) { continue; }
                var tokens = Tokenize(line).Select(s => s.Trim()).ToArray();

                bool LineParsed = false;
                foreach (var p in IPFParsers)
                {
                    if (p(tokens))
                    {
                        LineParsed = true;
                        break;
                    }
                }

                if (!LineParsed)
                {
                    Console.WriteLine("Error: Line {0} is invalid", i + 1);
                    break;
                }
            }
            return (Variables, Fees);
        }

        #region Tokenization
        IEnumerable<string> Tokenize(string input)
        {
            string token = string.Empty;
            bool inQuote = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '\'')
                {
                    inQuote = !inQuote;
                    if (!inQuote)
                    {
                        yield return token;
                        token = string.Empty;
                    }
                }
                else if (c == ' ' && !inQuote)
                {
                    if (token != string.Empty)
                    {
                        yield return token;
                        token = string.Empty;
                    }
                }
                else
                {
                    token += c;
                }
            }
            if (token != string.Empty)
            {
                yield return token;
            }
        }
        #endregion

        #region List Parsing
        bool ParseList(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.None) return false;
            if (tokens.Length != 5) return false;
            if (tokens[0] != "DEFINE") return false;
            if (tokens[1] != "LIST") return false;
            if (tokens[3] != "AS") return false;
            CurrentlyParsing = Parsing.List;
            CurrentList = new IPFVariableList(tokens[2], tokens[4], new List<IPFListItem>(), string.Empty);
            return true;
        }

        bool ParseListValue(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.List) return false;
            if (tokens.Length != 4) return false;
            if (tokens[0] != "VALUE") return false;
            if (tokens[2] != "AS") return false;
            var item = new IPFListItem(tokens[3], tokens[1]);
            CurrentList.Items.Add(item);
            return true;
        }

        bool ParseListDefaultValue(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.List) return false;
            if (tokens.Length != 2) return false;
            if (tokens[0] != "DEFAULT") return false;
            CurrentList = CurrentList with { DefaultSymbol = tokens[1] };
            return true;
        }
        #endregion

        #region Number Parsing
        bool ParseNumber(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.None) return false;
            if (tokens.Length != 5) return false;
            if (tokens[0] != "DEFINE") return false;
            if (tokens[1] != "NUMBER") return false;
            if (tokens[3] != "AS") return false;
            CurrentlyParsing = Parsing.Number;
            CurrentNumber = new IPFVariableNumber(tokens[2], tokens[4], int.MinValue, int.MaxValue, 0);
            return true;
        }

        bool ParseNumberBetween(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.Number) return false;
            if (tokens.Length != 4) return false;
            if (tokens[0] != "BETWEEN") return false;
            if (tokens[2] != "AND") return false;
            if (!int.TryParse(tokens[1], out int MinValue)) return false;
            if (!int.TryParse(tokens[3], out int MaxValue)) return false;
            CurrentNumber = CurrentNumber with { MinValue = MinValue, MaxValue = MaxValue };
            return true;
        }

        bool ParseNumberDefault(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.Number) return false;
            if (tokens.Length != 2) return false;
            if (tokens[0] != "DEFAULT") return false;
            if (!int.TryParse(tokens[1], out int DefaultValue)) return false;
            CurrentNumber = CurrentNumber with { DefaultValue = DefaultValue };
            return true;
        }
        #endregion

        #region Boolean Parsing
        bool ParseBoolean(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.None) return false;
            if (tokens.Length != 5) return false;
            if (tokens[0] != "DEFINE") return false;
            if (tokens[1] != "BOOLEAN") return false;
            if (tokens[3] != "AS") return false;
            CurrentlyParsing = Parsing.Boolean;
            CurrentBoolean = new IPFVariableBoolean(tokens[2], tokens[4], false);
            return true;
        }

        bool ParseBooleanDefault(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.Boolean) return false;
            if (tokens.Length != 2) return false;
            if (tokens[0] != "DEFAULT") return false;
            if (!bool.TryParse(tokens[1], out bool BooleanValue)) return false;
            CurrentBoolean = CurrentBoolean with { DefaultValue = BooleanValue };
            return true;
        }
        #endregion

        #region Parse EndDefine
        bool ParseEndDefine(string[] tokens)
        {
            if (tokens.Length != 1) return false;
            if (tokens[0] != "ENDDEFINE") return false;
            switch (CurrentlyParsing)
            {
                case Parsing.List:
                    Variables.Add(CurrentList);
                    CurrentlyParsing = Parsing.None;
                    return true;
                case Parsing.Boolean:
                    Variables.Add(CurrentBoolean);
                    CurrentlyParsing = Parsing.None;
                    return true;
                case Parsing.Number:
                    Variables.Add(CurrentNumber);
                    CurrentlyParsing = Parsing.None;
                    return true;
            }
            return false;
        }
        #endregion

        #region Parse Comment
        bool ParseComment(string[] tokens)
        {
            if (tokens.Length == 0) return false;
            if (!tokens[0].StartsWith("#")) return false;
            return true;
        }
        #endregion

        #region Fee Parsing
        bool ParseFee(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.None) return false;
            if (tokens.Length != 3) return false;
            if (tokens[0] != "COMPUTE") return false;
            if (tokens[1] != "FEE") return false;
            CurrentlyParsing = Parsing.Fee;
            CurrentFee = new IPFFee(tokens[2], new List<IPFItem>());
            return true;
        }

        bool ParseFeeCase(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.Fee) return false;
            if (tokens[0] != "CASE") return false;
            if (tokens[tokens.Length - 1] != "AS") return false;
            CurrentlyParsing = Parsing.FeeCase;
            var ConditionTokens = tokens.Skip(1).Take(tokens.Length - 2);
            CurrentFeeCase = new IPFFeeCase(ConditionTokens, new List<IPFFeeYield>());
            return true;
        }

        bool ParseFeeYield(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.FeeCase && CurrentlyParsing != Parsing.Fee) return false;
            if (tokens[0] != "YIELD") return false;
            // The yield value is comprised of all tokens until "IF" (will be evaluated)
            var ValueTokens = tokens.AsEnumerable().Skip(1).TakeWhile(w => !w.Equals("IF"));
            // The condition is comprised of all tokens until "IF"
            var ConditionTokens = tokens.AsEnumerable().Reverse().TakeWhile(w => !w.Equals("IF")).Reverse();
            var Yield = new IPFFeeYield(ConditionTokens, ValueTokens);
            CurrentFeeCase.Yields.Add(Yield);
            return true;
        }

        bool ParseFeeEndCase(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.FeeCase) return false;
            if (tokens.Length != 1) return false;
            if (tokens[0] != "ENDCASE") return false;
            CurrentlyParsing = Parsing.Fee;
            CurrentFee.Cases.Add(CurrentFeeCase);
            CurrentFeeCase = new IPFFeeCase(Enumerable.Empty<string>(), new List<IPFFeeYield>());
            return true;
        }

        bool ParseEndCompute(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.Fee) return false;
            if (tokens.Length != 1) return false;
            if (tokens[0] != "ENDCOMPUTE") return false;
            // Dump remaining yields (if any)
            if (CurrentFeeCase.Yields.Count > 0)
            {
                CurrentFee.Cases.Add(CurrentFeeCase);
            }
            Fees.Add(CurrentFee);
            CurrentFeeCase = new IPFFeeCase(Enumerable.Empty<string>(), new List<IPFFeeYield>());
            CurrentlyParsing = Parsing.None;
            return true;
        }
        #endregion
    }

    internal enum Parsing
    {
        None, List, Number, Boolean, Fee, FeeCase
    }

    public abstract record IPFVariable(string Name, string Text);
    public record IPFVariableBoolean(string Name, string Text, bool DefaultValue) : IPFVariable(Name, Text);
    public record IPFVariableList(string Name, string Text, IList<IPFListItem> Items, string DefaultSymbol) : IPFVariable(Name, Text);
    public record IPFListItem(string Symbol, string Value);
    public record IPFVariableNumber(string Name, string Text, int MinValue, int MaxValue, int DefaultValue) : IPFVariable(Name, Text);

    public abstract record IPFItem(IEnumerable<string> Condition);
    public record IPFFee(string Name, IList<IPFItem> Cases)
    {
        public override string ToString()
        {
            return string.Format("\n\rFEE: {0}\n\r{1}", Name, string.Join(Environment.NewLine, Cases));
        }
    }
    public record IPFFeeCase(IEnumerable<string> Condition, IList<IPFFeeYield> Yields) : IPFItem(Condition)
    {       
        public override string ToString()
        {
            return string.Format("CASE: {0}\n\r{1}", string.Join(" ", Condition), string.Join(Environment.NewLine, Yields));
        }
    }
    public record IPFFeeYield(IEnumerable<string> Condition, IEnumerable<string> Values) : IPFItem(Condition)
    {
        public override string ToString()
        {
            return string.Format("YIELD: {0} CONDITION: {1}", string.Join(" ", Values), string.Join(" ", Condition));
        }
    }
}
