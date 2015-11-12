using System;
using System.Collections.Generic;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Execution;


namespace ZCL.RTScript.Logic.Expression
{
    internal class RTFunction : RTExpression, IRTFunction
    {
        /// <summary>
        /// The name of the function in parsing time.
        /// This can be different from Metadata.FunctionName
        /// when the function name cannot be resolved at parsing time.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// This is loaded when the function is successfully parsed.
        /// </summary>
        public RTMetadata Metadata
        {
            get;
            private set;
        }

        /// <summary>
        /// All possible args:
        /// null; bool; number; literal; expression list; variable; function 
        /// </summary>
        public IList<object> Args
        {
            get;
            private set;
        }

        /// <summary>
        /// A function is supposed to be immutable after parsing.
        /// Do not modify it - what's done is done, it cannot be undone.
        /// </summary>
        public RTFunction()
        {
            this.Args = new List<object>();
        }

        #region parsing


        private enum ParsingState
        {
            Start,
            Invalid,
            FuncName,
            FuncArg,
            FuncArgNegative
        }

        protected override void ParseInternal(string srcText, RTParsingContext context)
        {
            bool flagExit = false;
            ParsingState state = ParsingState.Start;
            int index = SourceStartIndex;

            while (!flagExit)
            {
                int tokenStartIndex = index;
                TokenType token = ReadToken(srcText, ref index);
                if (token == TokenType.NONE) break;
                string tokenValue = srcText.Substring(tokenStartIndex, index - tokenStartIndex);

                switch (state)
                {
                    case ParsingState.Start:

                        if (token == TokenType.WHITESPACE)
                        {
                            //ignore
                        }
                        else if (token == TokenType.OPEN_BRACKET)
                        {
                            state = ParsingState.FuncName;
                        }
                        else
                        {
                            context.Errors.Add(new RTParsingError(tokenStartIndex, index - 1, RTErrorCode.OpenBracketMissing, ErrorMessages.Function_Open_Bracket_Missing));
                            flagExit = true;
                        }
                        break;

                    case ParsingState.Invalid:
                        if (token == TokenType.CLOSE_BRACKET) //exit at nearest closing bracket
                        {
                            flagExit = true;
                        }
                        break;

                    case ParsingState.FuncName:
                        if (token == TokenType.SYMBOL)
                        {
                            this.Name = tokenValue;
                            state = ParsingState.FuncArg;
                        }
                        else if (token == TokenType.IDENTIFIER)
                        {
                            bool isvalid = CheckIdentifier(tokenValue);
                            if (isvalid)
                            {
                                this.Name = tokenValue;
                                state = ParsingState.FuncArg;
                            }
                            else
                            {
                                state = ParsingState.Invalid;
                                context.Errors.Add(new RTParsingError(tokenStartIndex, index - 1, RTErrorCode.InvalidIdentifier, string.Format(ErrorMessages.Function_Invalid_Arg_Identifier, tokenValue)));
                            }
                        }
                        else
                        {
                            state = ParsingState.Invalid;
                            context.Errors.Add(new RTParsingError(tokenStartIndex, index - 1, RTErrorCode.InvalidFunctionName, string.Format(ErrorMessages.Function_Name_Missing, token)));
                            if (token == TokenType.CLOSE_BRACKET)
                                flagExit = false;
                        }
                        break;
                    case ParsingState.FuncArgNegative:
                        if (token == TokenType.WHITESPACE)
                        {
                            this.Args.Add(new RTVariable("-"));
                        }
                        else if (token == TokenType.NUMBER)
                        {
                            double temp;
                            if (double.TryParse(tokenValue, out temp))
                            {
                                this.Args.Add(-temp);
                            }
                            else
                            {
                                context.Errors.Add(new RTParsingError(tokenStartIndex, index - 1, RTErrorCode.InvalidNumber, string.Format(ErrorMessages.Function_Invalid_Arg_Number, tokenValue)));
                            }
                        }
                        else
                        {
                            context.Errors.Add(new RTParsingError(tokenStartIndex, index - 1, RTErrorCode.InvalidToken, string.Format(ErrorMessages.Function_Invalid_Arg, tokenValue)));
                            if (token == TokenType.CLOSE_BRACKET)
                                flagExit = false;
                        }
                        state = ParsingState.FuncArg;
                        break;

                    case ParsingState.FuncArg:
                        switch (token)
                        {
                            case TokenType.WHITESPACE://ignore
                                break;

                            case TokenType.CLOSE_BRACKET:
                                flagExit = true;
                                break;

                            case TokenType.OPEN_BRACKET:
                                RTFunction func = new RTFunction();
                                func.Parse(srcText, tokenStartIndex, context);
                                index = context.SourceEndIndex + 1;
                                this.Args.Add(func);
                                break;

                            case TokenType.TRUE:
                                this.Args.Add(true);
                                break;

                            case TokenType.FALSE:
                                this.Args.Add(false);
                                break;

                            case TokenType.NULL:
                                this.Args.Add(null);
                                break;

                            case TokenType.INVALID:
                                context.Errors.Add(new RTParsingError(tokenStartIndex, index - 1, RTErrorCode.InvalidToken, string.Format(ErrorMessages.Function_Invalid_Arg, tokenValue)));
                                break;

                            case TokenType.NUMBER:
                                double temp;
                                if (double.TryParse(tokenValue, out temp))
                                {
                                    this.Args.Add(temp);
                                }
                                else
                                {
                                    context.Errors.Add(new RTParsingError(tokenStartIndex, index - 1, RTErrorCode.InvalidNumber, string.Format(ErrorMessages.Function_Invalid_Arg_Number, tokenValue)));
                                }
                                break;

                            case TokenType.SYMBOL:
                                if (tokenValue == "-")
                                {
                                    state = ParsingState.FuncArgNegative;
                                }
                                else
                                {
                                    this.Args.Add(new RTVariable(tokenValue));
                                }
                                break;

                            case TokenType.IDENTIFIER:
                                bool isValid = CheckIdentifier(tokenValue);
                                if (isValid)
                                {
                                    this.Args.Add(new RTVariable(tokenValue));
                                }
                                else
                                {
                                    context.Errors.Add(new RTParsingError(tokenStartIndex, index - 1, RTErrorCode.InvalidIdentifier, string.Format(ErrorMessages.Function_Invalid_Arg_Identifier, tokenValue)));
                                }
                                break;

                            case TokenType.EXPR_START:
                                RTExpression expr = new RTExpressionList();
                                expr.Parse(srcText, tokenStartIndex, context);
                                index = context.SourceEndIndex + 1;
                                Args.Add(expr);
                                break;
                            default:
                                throw new NotSupportedException("Unknown token type.");

                        }
                        break;
                    default:
                        throw new NotSupportedException("Unknow Function Parsing State.");
                }

            }
            if (!flagExit)
            {
                context.Errors.Add(new RTParsingError(this.SourceStartIndex, index - 1, RTErrorCode.EndBraketMissing, ErrorMessages.Function_End_Bracket_Missing));
            }
            context.SourceEndIndex = index - 1;

            //resolve metadata
            this.Metadata = this.Name == null ? null : context.MetadataFactory.GetMetadata(this.Name);
            if (this.Metadata == null)
            {
                this.Metadata = context.MetadataFactory.GetMetadata(RTSysSymbols.APPLY); //the apply symbol
                this.Args.Insert(0, new RTVariable(this.Name));
                this.Name = RTSysSymbols.APPLY;
            }
            this.Metadata.NormalizeArgs(this.Args);

#if DEBUG
            _src = srcText.Substring(this.SourceStartIndex, context.SourceEndIndex - this.SourceStartIndex + 1);
#endif
        }

#if DEBUG
        //debugging facility
        private string _src; //this

        public override string ToString()
        {
            return _src;
        }
#endif


        private enum TokenType
        {
            NONE,
            OPEN_BRACKET,
            CLOSE_BRACKET,
            NUMBER,
            IDENTIFIER,
            TRUE,
            FALSE,
            EXPR_START,
            SYMBOL,
            WHITESPACE,
            NULL,
            INVALID
        }

        /// <summary>
        /// system operators:
        /// => function apply
        /// :: define
        /// </summary>
        /// <param name="srcText"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static TokenType ReadToken(string srcText, ref int index)
        {
            if (index >= srcText.Length) return TokenType.NONE;

            if (char.IsWhiteSpace(srcText[index]))
            {
                while (++index < srcText.Length && char.IsWhiteSpace(srcText[index])) ;
                return TokenType.WHITESPACE;
            }
            else if (srcText[index] == EXPR_FUNC_OPEN)
            {
                index++;
                return TokenType.OPEN_BRACKET;
            }
            else if (srcText[index] == EXPR_FUNC_CLOSE)
            {
                index++;
                return TokenType.CLOSE_BRACKET;
            }
            else if ("+-*/%^=!#$&|~?\\:<>".IndexOf(srcText[index]) != -1)
            {
                if (srcText[index] == '=' && index + 1 < srcText.Length && srcText[index + 1] == '>') //capture =>
                {
                    index++;
                }
                else if (srcText[index] == ':' && index + 1 < srcText.Length && (srcText[index + 1] == ':' || srcText[index + 1] == '=')) //capture ::, :=
                {
                    index++;
                }
                else if (srcText[index] == '!' && index + 1 < srcText.Length && srcText[index + 1] == '=') //capture !=
                {
                    index++;
                }
                else if ("<>".IndexOf(srcText[index]) != -1 && index + 1 < srcText.Length && srcText[index + 1] == '=') //capture >= <=
                {
                    index++;
                }
                index++;
                return TokenType.SYMBOL;
            }
            else if (char.IsDigit(srcText[index]) || srcText[index] == '.')
            {
                while (++index < srcText.Length && !char.IsWhiteSpace(srcText[index]) && srcText[index] != EXPR_FUNC_CLOSE) ;
                return TokenType.NUMBER;
            }
            else if (srcText[index] == EXPR_MARKER)
            {
                if (++index < srcText.Length && srcText[index] == EXPR_START)
                {
                    index++;
                    return TokenType.EXPR_START;
                }
                while ((!char.IsWhiteSpace(srcText[index]) && srcText[index] != EXPR_FUNC_CLOSE)) index++;
                return TokenType.INVALID;
            }
            else if (char.IsLetter(srcText[index]) || srcText[index] == '_')
            {
                int startIndex = index;
                while (++index < srcText.Length && !char.IsWhiteSpace(srcText[index]) && srcText[index] != EXPR_FUNC_CLOSE) ;
                var identifier = srcText.Substring(startIndex, index - startIndex);
                if (identifier == "true") return TokenType.TRUE; //check for keywords
                else if (identifier == "false") return TokenType.FALSE;
                else if (identifier == "null") return TokenType.NULL;
                return TokenType.IDENTIFIER;
            }

            else
            {
                while (++index < srcText.Length && (!char.IsWhiteSpace(srcText[index]) && srcText[index] != EXPR_FUNC_CLOSE)) ;
                return TokenType.INVALID;
            }

        }

        private static bool CheckIdentifier(string p)
        {
            if (p.Length == 0) return false;

            if (!(p[0] == '_' || char.IsLetterOrDigit(p[0])))
            {
                return false;
            }

            for (int i = 1; i < p.Length; i++)
            {
                if (!(char.IsLetterOrDigit(p[i]) || p[i] != '_' || p[i] != '.' || p[i] != '$')) return false;
            }

            return true;
        }

        #endregion

        public override object Execute(RTExecutionContext context)
        {
            var scope = new RTScope(this);
            var retval = scope.Execute(context);

            if (!Metadata.HasReturnValue)
            {
                retval = RTVoid.Singleton;
            }
            return retval;
        }

    }

}
