using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace RexReplace.GUI.Logic
{
    /// <summary>
    /// An instance of this class is used by the <seealso cref="RuleSet"/> class to merge a regex match and a replacement template string into a replacement text.
    /// This class will only return the first error.
    /// </summary>
    internal class ReplaceExpression : RexReplace.GUI.Logic.IReplaceContext
    {
        public int MatchId
        {
            get;
            private set;
        }

        public int MatchCount
        {
            get;
            private set;
        }

        public GroupCollection MatchValues
        {
            get;
            private set;
        }

        private StringBuilder buffer = new StringBuilder();

        internal string Execute(string expr, GroupCollection values, int j, int count)
        {
            //init
            MatchId = j;
            MatchValues = values;
            MatchCount = count;
            _inExpr = false;
            buffer.Clear();
            _exprDepth = 0;
            int index = 0;

            while (true)
            {
                int startIndex = index;
                if (startIndex >= expr.Length) break;

                TokenType token = ReadToken(expr, ref index);
                Debug.Assert(token == TokenType.None); //other types are handled in expression
                if (index > startIndex)
                {
                    AppendEscape(expr, startIndex, index - startIndex, '{');
                }

                if (_inExpr) //next token is {
                {
                    var retval = Execute(expr, ref index);
                    buffer.Append(retval);
                }
            }

            return buffer.ToString();
        }

        private void AppendEscape(string expr, int startIndex, int length, char escapeChar)
        {
            int nextStart = startIndex + length;
            bool escape = false;
            for (int i = startIndex; i < nextStart; i++)
            {
                if (escape)
                {
                    if (expr[i] != escapeChar)
                    {
                        buffer.Append(expr[i]);
                    }
                    escape = false;

                }
                else
                {
                    if (expr[i] == escapeChar)
                    {
                        escape = true;
                    }

                    buffer.Append(expr[i]);
                }
            }
        }

        private object Execute(string expr, ref int index)
        {
            List<object> parms = new List<object>();
            ParsingState state = ParsingState.Start;
            object literal;
            int exprStartIndex = index;

            while (true) //get all parms
            {
                int tokenStartIndex = index;
                if (tokenStartIndex >= expr.Length) throw new RuleException("Unexpected end of expression") { SourceCode = expr, SourceIndex = tokenStartIndex };
                TokenType token = ReadToken(expr, ref index);
                if (state == ParsingState.Start)
                {
                    Debug.Assert(token == TokenType.ExprStart);
                    state = ParsingState.FuncName;
                }
                else if (state == ParsingState.FuncName)
                {
                    if (token == TokenType.WhiteSpace) continue;
                    if (token != TokenType.Literal) throw new RuleException("Expecting a function name at the beginning of an expression") { SourceCode = expr, SourceIndex = tokenStartIndex };
                    try
                    {
                        ParseLiteral(expr.Substring(tokenStartIndex, index - tokenStartIndex), out literal);
                    }
                    catch (RuleException ex)
                    {
                        ex.SourceCode = expr;
                        ex.SourceIndex = tokenStartIndex;
                        throw;
                    }
                    parms.Add(literal);
                    state = ParsingState.ParmStart;
                }
                else if (state == ParsingState.ParmStart)
                {
                    if (token == TokenType.WhiteSpace) continue;
                    if (token == TokenType.ExprEnd) break;

                    if (token == TokenType.String || token == TokenType.Literal)
                    {
                        try
                        {
                            ParseLiteral(expr.Substring(tokenStartIndex, index - tokenStartIndex), out literal);
                        }
                        catch (RuleException ex)
                        {
                            ex.SourceCode = expr;
                            ex.SourceIndex = tokenStartIndex;
                            throw;
                        }
                        parms.Add(literal);
                        state = ParsingState.ParmEnd;
                    }
                    else if (token == TokenType.ExprStart)
                    {
                        index = tokenStartIndex;
                        _exprDepth--;
                        literal = Execute(expr, ref index);
                        parms.Add(literal);
                        state = ParsingState.ParmEnd;
                    }
                    else
                    {
                        Debug.Assert(false);
                    }

                }
                else if (state == ParsingState.ParmEnd)
                {
                    if (token == TokenType.ExprEnd) break;
                    if (token == TokenType.WhiteSpace) continue;
                    if (token != TokenType.Separator) throw new RuleException("Expecting a separator") { SourceCode = expr, SourceIndex = tokenStartIndex };
                    state = ParsingState.ParmStart;
                }
            }

            try
            {
                return Execute(parms);
            }
            catch (Exception ex)
            {
                throw new RuleException(ex.Message, ex) { SourceCode = expr, SourceIndex = exprStartIndex };
                throw ex;
            }
        }

        private object Execute(List<object> parms)
        {
            if (parms[0] is double)
            {
                int index = Convert.ToInt32(parms[0]);
                if (index >= MatchValues.Count) throw new RuleException("Index '" + index + "' is out of range for matched value accessor");
                return MatchValues[index].Value;
            }

            string funcName = parms[0] as string;
            if (funcName != null && funcName[0] != '"')
            {
                ReplaceOp func;
                if (FUNCTIONS.TryGetValue(funcName, out func))
                {
                    if (parms.Count - 1 < func.MinArgNumber) throw new RuleException("Function {OpName} needs at least {MinArgNumber} arguments but got {ActualArgNumber} in expression") { OpName = funcName, MinArgNumber = func.MinArgNumber, ActualArgNumber = parms.Count - 1 };
                    if (parms.Count - 1 > func.MaxArgNumber) throw new RuleException("Function {OpName} can have at most {MaxArgNumber} arguments but got {ActualArgNumber} in expression") { OpName = funcName, MaxArgNumber = func.MaxArgNumber, ActualArgNumber = parms.Count - 1 };
                    return func.Body(parms, this);
                }
            }

            throw new RuleException("Function name {OpName} is not found in expression") { OpName = funcName };
        }

        private static readonly Dictionary<string, ReplaceOp> FUNCTIONS = new Dictionary<string, ReplaceOp>();

        private void ParseLiteral(string rawValue, out object literal)
        {
            Debug.Assert(rawValue.Length > 0);
            if (rawValue[0] == '"') //check string
            {
                int endQuotes = 0;
                int index = rawValue.Length - 1;
                while (index > 0 && rawValue[index--] == '"') endQuotes++;
                if (endQuotes % 2 != 1) throw new RuleException("String <" + rawValue + "> has missing end quote.");
                literal = rawValue.Substring(1, rawValue.Length - 2).Replace("\"\"", "\"");
                return;
            }

            //true, false
            rawValue = rawValue.TrimEnd();
            Debug.Assert(rawValue.Length > 0);
            if (rawValue == "true") literal = true;
            else if (rawValue == "false") literal = false;
            else
            {
                double retval;
                if (double.TryParse(rawValue, out retval))
                    literal = retval;
                else
                    literal = rawValue;
            }
        }

        private enum ParsingState
        {
            Start,
            FuncName,
            ParmStart,
            ParmEnd
        }

        private bool _inExpr = false;
        private int _exprDepth = 0;

        private TokenType ReadToken(string expr, ref int index)
        {
            if (!_inExpr)
            {
                bool lastIsExprStart = false;
                while (true)
                {
                    if (index >= expr.Length) return TokenType.None;
                    if (expr[index] == '{')
                    {
                        lastIsExprStart = !lastIsExprStart;
                    }
                    else
                    {
                        if (lastIsExprStart)
                        {
                            index--;
                            _inExpr = true;
                            return TokenType.None;
                        }
                    }
                    index++;
                }
            }
            else
            {
                if (index >= expr.Length) return TokenType.None;
                if (expr[index] == '{')
                {
                    index++;
                    _exprDepth++;
                    return TokenType.ExprStart;
                }
                if (expr[index] == '}')
                {
                    index++;
                    _exprDepth--;
                    if (_exprDepth == 0) _inExpr = false;
                    return TokenType.ExprEnd;
                }
                if (expr[index] == ',')
                {
                    index++;
                    return TokenType.Separator;
                }
                if (char.IsWhiteSpace(expr[index]))
                {
                    do
                    {
                        index++;
                    }
                    while (index < expr.Length && char.IsWhiteSpace(expr[index]));
                    return TokenType.WhiteSpace;
                }
                if (expr[index] == '"')
                {
                    bool lastIsQuote = false;
                    while (true)
                    {
                        index++;
                        if (index >= expr.Length) return TokenType.String;
                        if (expr[index] == '"')
                        {
                            lastIsQuote = !lastIsQuote;
                        }
                        else
                        {
                            if (lastIsQuote) return TokenType.String;
                        }
                    }
                }

                do
                {
                    index++;
                }
                while (index < expr.Length && "{},".IndexOf(expr[index]) == -1 && !char.IsWhiteSpace(expr[index]));
                return TokenType.Literal;
            }
        }

        private enum TokenType
        {
            ExprStart,
            ExprEnd,
            Literal,
            String,
            Separator,
            WhiteSpace,
            None
        }

        static ReplaceExpression()
        {
            FUNCTIONS.Add("matchCount", new ReplaceOp((x, y) => { return y.MatchCount; }, 0, 0));

            FUNCTIONS.Add("matchId", new ReplaceOp((x, y) => { return y.MatchId; }, 0, 0));

            FUNCTIONS.Add("neg", new ReplaceOp((x, y) =>
            {
                try
                {
                    return -Convert.ToDouble(x[1]);
                }
                catch
                {
                    return double.NaN;
                }

            }));

            FUNCTIONS.Add("sum", new ReplaceOp((x, y) =>
            {
                double sum = 0;
                for (int i = 1; i < x.Count; i++)
                {
                    try
                    {
                        sum += Convert.ToDouble(x[i]);
                    }
                    catch
                    {
                        return double.NaN;
                    }
                }
                return sum;
            }, 0, int.MaxValue));

            FUNCTIONS.Add("mul", new ReplaceOp((x, y) =>
            {
                double prod = 1;
                for (int i = 1; i < x.Count; i++)
                {
                    try
                    {
                        prod *= Convert.ToDouble(x[i]);
                    }
                    catch
                    {
                        return double.NaN;
                    }
                }
                return prod;
            }, 0, int.MaxValue));

            FUNCTIONS.Add("div", new ReplaceOp((x, y) =>
            {
                try
                {
                    double parm1 = Convert.ToDouble(x[1]);
                    double parm2 = Convert.ToDouble(x[2]);
                    return parm1 / parm2;
                }
                catch
                {
                    return double.NaN;
                }

            }, 2, 2));

            FUNCTIONS.Add("toUpper", new ReplaceOp((x, y) => { return x[1].ToString().ToUpper(); }));

            FUNCTIONS.Add("toLower", new ReplaceOp((x, y) => { return x[1].ToString().ToLower(); }));

            //always return double for number
            FUNCTIONS.Add("length", new ReplaceOp((x, y) => Convert.ToDouble(x[1].ToString().Length)));

            FUNCTIONS.Add("trim", new ReplaceOp((x, y) => x[1].ToString().Trim()));

            FUNCTIONS.Add("trimEnd", new ReplaceOp((x, y) => x[1].ToString().TrimEnd()));

            FUNCTIONS.Add("trimStart", new ReplaceOp((x, y) => x[1].ToString().TrimStart()));

            FUNCTIONS.Add("toUpperFirstChar", new ReplaceOp((x, y) =>
            {
                string str = x[1].ToString();
                if (str.Length == 0) return str;
                return Char.ToUpper(str[0]).ToString() + (str.Length > 1 ? str.Substring(1) : string.Empty);
            }));

            FUNCTIONS.Add("toLowerFirstChar", new ReplaceOp((x, y) =>
            {
                string str = x[1].ToString();
                if (str.Length == 0) return str;
                return Char.ToLower(str[0]).ToString() + (str.Length > 1 ? str.Substring(1) : string.Empty);
            }));

            FUNCTIONS.Add("substring", new ReplaceOp((x, y) =>
            {
                string str = x[1].ToString();
                int startIndex = Convert.ToInt32(x[2]);
                int length = -1;
                if (x.Count > 2)
                {
                    length = Convert.ToInt32(x[3]);
                }

                if (length == -1) return str.Substring(startIndex);
                else return str.Substring(startIndex, length);
            }, 2, 3));

            FUNCTIONS.Add("indexOf", new ReplaceOp((x, y) =>
            {
                string str1 = x[1].ToString();
                string str2 = x[2].ToString();
                int startIndex = 0;

                if (x.Count > 3)
                {
                    startIndex = Convert.ToInt32(x[3]);
                }

                return str1.IndexOf(str2, startIndex);
            }, 2, 3));

            FUNCTIONS.Add("if", new ReplaceOp((x, y) =>
            {
                string str1 = x[1].ToString();
                if (str1 == string.Empty || (str1.Length == 5 && str1.ToLower() == "false"))
                    return x[3];
                else return x[2];
            }, 3, 3));

            FUNCTIONS.Add("equal", new ReplaceOp((x, y) =>
            {
                string str1 = x[1].ToString();
                string str2 = x[2].ToString();
                return str1 == str2;
            }, 2, 2));

            FUNCTIONS.Add("concat", new ReplaceOp((x, y) =>
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < x.Count; i++)
                {
                    sb.Append(x[i].ToString());
                }
                return sb.ToString();
            }, 0, int.MaxValue));

            FUNCTIONS.Add("replace", new ReplaceOp((x, y) =>
            {
                string str1 = x[1].ToString();
                string str2 = x[2].ToString();
                string str3 = x[3].ToString();
                return str1.Replace(str2, str3);
            }, 3, 3));

            FUNCTIONS.Add("asInt", new ReplaceOp((x, y) =>
            {
                return int.Parse(x[1].ToString());
            }));

            FUNCTIONS.Add("asBool", new ReplaceOp((x, y) =>
            {
                return StringToBool(x[1].ToString());
            }));

            FUNCTIONS.Add("asDouble", new ReplaceOp((x, y) =>
            {
                return double.Parse(x[1].ToString());
            }));

            FUNCTIONS.Add("not", new ReplaceOp((x, y) =>
            {
                return !StringToBool(x[1].ToString());
            }));

            FUNCTIONS.Add("calc", new ReplaceOp((x, y) =>
            {
                try
                {
                    return 0;// calculator.Compute(x[1].ToString());
                }
                catch
                {
                    return string.Format("[invalid calc expression: \"{0}\"]",  x[1].ToString().Replace("\"", "\\\""));
                }
            }));

            FUNCTIONS.Add("rand", new ReplaceOp((x, y) => {

                if (x.Count < 2)
                    return rand.Next();

                var upperBound = Convert.ToInt32(x[1]);
                if (x.Count < 3)
                    return rand.Next(upperBound);

                var lowerBound = upperBound;
                upperBound = Convert.ToInt32(x[2]);
                return rand.Next(lowerBound, upperBound);

            }, 0, 2));
        }

        private static Random rand = new Random();

        
      //  private static ICalculator calculator = new CalculatorFactory().createInstance();

        private static bool StringToBool(String value)
        {
            if (String.IsNullOrEmpty(value))
                return false;

            var lowerValue = value.ToLower();
            if (lowerValue == "false")
                return false;

            double number;
            if (!double.TryParse(value, out number))
                return true;

            if (number == 0d)
                return false;

            return true;
        }
    }

}
