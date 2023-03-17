using System.Text;
using System.Xml.Linq;

namespace IPFees.Parser
{
    public class DslParser : IDslParser
    {
        Parsing CurrentlyParsing = Parsing.None;

        private DslVariableList CurrentList { get; set; } = new DslVariableList(string.Empty, string.Empty, new List<DslListItem>(), string.Empty, false);
        private DslVariableNumber CurrentNumber { get; set; } = new DslVariableNumber(string.Empty, string.Empty, int.MinValue, int.MaxValue, 0);
        private DslVariableBoolean CurrentBoolean { get; set; } = new DslVariableBoolean(string.Empty, string.Empty, false);
        private DslFee CurrentFee { get; set; } = new DslFee(string.Empty, false, new List<DslItem>(), new List<DslFeeVar>());
        private DslFeeCase CurrentFeeCase { get; set; } = new DslFeeCase(Enumerable.Empty<string>(), new List<DslFeeYield>());

        private IList<DslVariable> IPFVariables = new List<DslVariable>();
        private IList<DslFee> IPFFees = new List<DslFee>();
        private IList<(DslError, string)> IPFErrors = new List<(DslError, string)>();


        public DslParser() { }

        public bool Parse(string source)
        {
            Func<string[], bool>[] IPFParsers = new Func<string[], bool>[]
            {
                ParseList,
                ParseListChoice,
                ParseListDefaultValue,
                ParseNumber,
                ParseNumberBetween,
                ParseNumberDefault,
                ParseBoolean,
                ParseBooleanDefault,
                ParseEndDefine,
                ParseFee,
                ParseFeeCase,
                ParseFeeYield,
                ParseFeeLet,
                ParseFeeEndCase,
                ParseEndCompute
            };

            string[] IPFData = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < IPFData.Length; i++)
            {
                // Ignore comments (after the # sign)
                int CommentIndex = IPFData[i].IndexOf('#');
                string line = (CommentIndex == -1 ? IPFData[i] : IPFData[i].Substring(0, CommentIndex)).Trim();
                // Do not process an empty line
                if (string.IsNullOrEmpty(line)) { continue; }
                // Generate tokens
                var tokens = Tokenize(line).Select(s => s.Trim()).ToArray();
                // Using the chain of command design pattern, try to decode the tokens from the line
                bool LineParsed = false;
                foreach (var p in IPFParsers)
                {
                    try
                    {
                        if (p(tokens))
                        {
                            LineParsed = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        IPFErrors.Add((DslError.SyntaxError, $"[Line {i + 1}] Error: {ex.Message}"));
                    }
                }
                // If the line hasn't been parsed until now, it's something wrong with it
                if (!LineParsed)
                {
                    IPFErrors.Add((DslError.SyntaxError, $"[Line {i + 1}] Error: Invalid syntax"));
                }
            }

            if (IPFErrors.Count > 0) return false;

            // Perform semantic checking
            var errs = DslSemanticChecker.Check(IPFVariables, IPFFees);
            if (errs.Any())
            {
                foreach (var e in errs) IPFErrors.Add(e);
                return false;
            }

            return true;
        }

        public IEnumerable<DslVariable> GetVariables()
        {
            if (IPFErrors.Count > 0) throw new NotSupportedException("Unable to access variables. Check the error list.");
            return IPFVariables;
        }

        public IEnumerable<DslFee> GetFees()
        {
            if (IPFErrors.Count > 0) throw new NotSupportedException("Unable to access fees. Check the error list.");
            return IPFFees;
        }
        public IEnumerable<(DslError, string)> GetErrors()
        {
            return IPFErrors;
        }

        #region Tokenization
        /// <summary>
        /// Tokenize input string, taking care of the single quote strings
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Tokens resulting from the input string</returns>
        IEnumerable<string> Tokenize(string input)
        {
            string token = string.Empty;
            var SingleCharTokens = new List<char> { '(', ')', '+', '-', '*', '/' };
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
                else if (SingleCharTokens.Contains(c) && !inQuote)
                {
                    if (token != string.Empty)
                    {
                        yield return token;
                        token = string.Empty;
                    }
                    yield return c.ToString();
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
            if (tokens.Length != 5 && tokens.Length != 6) return false;
            if (tokens[0] != "DEFINE") return false;
            if (tokens[1] != "LIST") return false;
            if (tokens[3] != "AS") return false;
            var IsMultiple = false;
            if (tokens.Length == 6)
            {
                if (tokens[5] != "MULTIPLE") return false;
                IsMultiple = true;
            }
            CurrentlyParsing = Parsing.List;
            CurrentList = new DslVariableList(tokens[2], tokens[4], new List<DslListItem>(), string.Empty, IsMultiple);
            return true;
        }

        bool ParseListChoice(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.List) return false;
            if (tokens.Length != 4) return false;
            if (tokens[0] != "CHOICE") return false;
            if (tokens[2] != "AS") return false;
            var item = new DslListItem(tokens[1], tokens[3]);
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
            CurrentNumber = new DslVariableNumber(tokens[2], tokens[4], int.MinValue, int.MaxValue, 0);
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
            CurrentBoolean = new DslVariableBoolean(tokens[2], tokens[4], false);
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
                    IPFVariables.Add(CurrentList);
                    CurrentlyParsing = Parsing.None;
                    return true;
                case Parsing.Boolean:
                    IPFVariables.Add(CurrentBoolean);
                    CurrentlyParsing = Parsing.None;
                    return true;
                case Parsing.Number:
                    IPFVariables.Add(CurrentNumber);
                    CurrentlyParsing = Parsing.None;
                    return true;
            }
            return false;
        }
        #endregion

        #region Fee Parsing
        bool ParseFee(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.None) return false;
            if (tokens.Length != 3 && tokens.Length != 4) return false;
            if (tokens[0] != "COMPUTE") return false;
            if (tokens[1] != "FEE") return false;
            CurrentlyParsing = Parsing.Fee;
            var IsFeeOptional = (tokens.Length == 4 && tokens[3] == "OPTIONAL");
            CurrentFee = new DslFee(tokens[2], IsFeeOptional, new List<DslItem>(), new List<DslFeeVar>());
            return true;
        }

        bool ParseFeeCase(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.Fee) return false;
            if (tokens[0] != "CASE") return false;
            if (tokens[tokens.Length - 1] != "AS") return false;
            CurrentlyParsing = Parsing.FeeCase;
            var ConditionTokens = tokens.Skip(1).Take(tokens.Length - 2);
            CurrentFeeCase = new DslFeeCase(ConditionTokens, new List<DslFeeYield>());
            return true;
        }

        bool ParseFeeYield(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.FeeCase && CurrentlyParsing != Parsing.Fee) return false;
            if (tokens[0] != "YIELD") return false;
            // The yield value is comprised of all tokens until "IF" (will be evaluated)
            var ValueTokens = tokens.AsEnumerable().Skip(1).TakeWhile(w => !w.Equals("IF"));
            // The condition is comprised of all tokens until "IF" or is TRUE if no IF token is found
            if (tokens.Any(a => a.Equals("IF")))
            {
                var ConditionTokens = tokens.AsEnumerable().Reverse().TakeWhile(w => !w.Equals("IF")).Reverse();
                var Yield = new DslFeeYield(ConditionTokens, ValueTokens);
                CurrentFeeCase.Yields.Add(Yield);
            }
            else
            {
                var Yield = new DslFeeYield(Enumerable.Empty<string>(), ValueTokens);
                CurrentFeeCase.Yields.Add(Yield);
            }
            return true;
        }

        bool ParseFeeLet(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.Fee) return false;
            if (tokens[0] != "LET") return false;
            if (tokens[2] != "AS") return false;
            var VarName = new StringBuilder().AppendFormat($"{CurrentFee.Name}.{tokens[1]}").ToString();
            var ValueTokens = tokens.AsEnumerable().Skip(3);
            CurrentFee.Vars.Add(new DslFeeVar(VarName, ValueTokens));
            return true;
        }

        bool ParseFeeEndCase(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.FeeCase) return false;
            if (tokens.Length != 1) return false;
            if (tokens[0] != "ENDCASE") return false;
            CurrentlyParsing = Parsing.Fee;
            CurrentFee.Cases.Add(CurrentFeeCase);
            CurrentFeeCase = new DslFeeCase(Enumerable.Empty<string>(), new List<DslFeeYield>());
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
            IPFFees.Add(CurrentFee);
            CurrentFeeCase = new DslFeeCase(Enumerable.Empty<string>(), new List<DslFeeYield>());
            CurrentlyParsing = Parsing.None;
            return true;
        }
        #endregion
    }

    internal enum Parsing
    {
        None, List, Number, Boolean, Fee, FeeCase
    }
}
