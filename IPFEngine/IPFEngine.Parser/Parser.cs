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
        private IList<IPFVariable> Variables { get; set; } = new List<IPFVariable>();

        public IPFParser(string[] source) => IPFData = source;

        public IEnumerable<IPFVariable> Parse()
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
                ParseEnd,
                ParseComment
            };

            for (int i = 0; i < IPFData.Length; i++)
            {
                string line = IPFData[i];
                if (line == string.Empty) { continue; }
                var tokens = Tokenize(line).ToArray();

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
            return Variables;
        }

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
        #region List Parsing
        bool ParseList(string[] tokens)
        {
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
            CurrentList.Values.Add(item);
            return true;
        }

        bool ParseListDefaultValue(string[] tokens)
        {
            if (CurrentlyParsing != Parsing.List) return false;
            if (tokens.Length != 2) return false;
            if (tokens[0] != "DEFAULT") return false;
            CurrentList = CurrentList with { DefaultValue = tokens[1] };
            return true;
        }
        #endregion

        #region Number Parsing
        bool ParseNumber(string[] tokens)
        {
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

        #region Parse End
        bool ParseEnd(string[] tokens)
        {
            if (tokens.Length != 1) return false;
            if (tokens[0] != "END") return false;
            switch (CurrentlyParsing)
            {
                case Parsing.None: return false;
                case Parsing.List:
                    Console.WriteLine("Adding list: {0}", CurrentList);
                    Variables.Add(CurrentList);
                    CurrentlyParsing = Parsing.None;
                    return true;
                case Parsing.Boolean:
                    Console.WriteLine("Adding boolean: {0}", CurrentBoolean);
                    Variables.Add(CurrentBoolean);
                    CurrentlyParsing = Parsing.None;
                    return true;
                case Parsing.Number:
                    Console.WriteLine("Adding number: {0}", CurrentNumber);
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
    }

    internal enum Parsing
    {
        None, List, Number, Boolean
    }

    public abstract record IPFVariable(string Name, string Text);
    public record IPFVariableBoolean(string Name, string Text, bool DefaultValue) : IPFVariable(Name, Text);
    public record IPFVariableList(string Name, string Text, IList<IPFListItem> Values, string DefaultValue) : IPFVariable(Name, Text);
    public record IPFListItem(string Symbol, string Value);
    public record IPFVariableNumber(string Name, string Text, int MinValue, int MaxValue, int DefaultValue) : IPFVariable(Name, Text);
}
