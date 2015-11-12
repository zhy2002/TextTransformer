using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    public abstract class RTLibMetadata : RTMetadata
    {
        /// <summary>
        /// The minimum number of args
        /// </summary>
        public virtual int MinArgNumber
        {
            get;
            protected set;
        }

        /// <summary>
        /// The maximum number of args
        /// </summary>
        public virtual int MaxArgNumber
        {
            get;
            protected set;
        }

        /// <summary>
        /// If you can pass a list as its argument
        /// </summary>
        public virtual bool HandlesList
        {
            get;
            protected set;
        }

        /// <summary>
        /// If this function accepts scope parameters
        /// </summary>
        public virtual bool HandlesScope
        {
            get;
            protected set;
        }

        public RTLibMetadata()
        {
            this.MaxArgNumber = int.MaxValue;
            this.HandlesList = true;
            this.HandlesScope = true;
        }

        public override object Execute(RTExecutionContext context)
        {
            RTScope scope = context.CurrentScope;
            int previousErrorCount = context.Errors.Count;
            var evaledArgs = new List<object>();
            bool allArgsResolved = EvaluateArgs(context, evaledArgs);

            //check
            if (evaledArgs.Count < this.MinArgNumber)
            {
                context.Errors.Add(new RTExecutionError(scope, RTErrorCode.Metadata_Not_Enough_Args, string.Format(ErrorMessages.Metadata_Not_Enough_Args, this.MinArgNumber, evaledArgs.Count)));
            }

            if (evaledArgs.Count > this.MaxArgNumber)
            {
                context.Errors.Add(new RTExecutionError(scope, RTErrorCode.Metadata_Too_Many_Args, string.Format(ErrorMessages.Metadata_Too_Many_Args, this.MaxArgNumber, evaledArgs.Count)));
            }

            if (context.Errors.Count > previousErrorCount)
            {
                return RTVoid.Singleton;
            }

            if (allArgsResolved)
            {
                return ExecuteList(evaledArgs);
            }
            else
            {
                if (!this.HandlesScope)
                {
                    int i = -1;
                    while (!(evaledArgs[++i] is RTScope)) ; //find the first unresolved scope
                    context.Errors.Add(new RTExecutionError(scope, RTErrorCode.Metadata_Invalid_Arg, string.Format(ErrorMessages.Metadata_Invalid_Arg_Func, i, scope.FunctionName)));
                    return RTVoid.Singleton;
                }
                else
                {
                    for (int i = 0; i < evaledArgs.Count; i++)
                    {
                        scope.AddArg(evaledArgs[i]);
                    }
                    return scope;
                }
            }
        }

        protected virtual bool EvaluateArgs(RTExecutionContext context, IList<object> evaledArgs)
        {
            RTScope func = context.CurrentScope;
            bool allArgsResolved = true;

            for (int i = 0; i < func.ArgCount; i++)
            {
                var evaled = EvaluateArg(i, context);
                RTScope evaledScope = evaled as RTScope;

                if (evaled is RTVoid) continue;

                if (evaled is RTScope || evaled is RTVariable)
                {
                    allArgsResolved = false;
                }

                if (!this.HandlesList && !(evaled is string) && evaled is IEnumerable)
                {
                    context.Errors.Add(new RTExecutionError(func, RTErrorCode.Metadata_Invalid_Arg, string.Format(ErrorMessages.Metadata_Invalid_Arg_List, i, func.FunctionName)));
                }

                evaledArgs.Add(evaled);
            }

            return allArgsResolved;
        }

        private object ExecuteList(IList<object> args)
        {
            if (!this.HandlesList)
            {
                return Execute(args);
            }

            IList<object> result = new List<object>();
            IEnumerator[] enumerators = new IEnumerator[args.Count];
            for (int i = 0; i < args.Count; i++)
            {
                enumerators[i] = GetEnumerator(args[i]);

                if (i != 0)
                {
                    bool moveFlag = enumerators[i].MoveNext();
                    Debug.Assert(moveFlag); //has at least 1 item
                }
            }

            while (MoveNext(enumerators))
            {
                for (int i = 0; i < args.Count; i++)
                {
                    args[i] = enumerators[i].Current;
                }
                var retval = Execute(args);
                result.Add(retval);
            }

            if (result.Count == 1) return result[0];
            else return result;
        }

        protected abstract object Execute(IList<object> tupple);

        private class SingletonEnumerator : IEnumerator
        {
            private object _value;
            private bool _isReset;

            internal SingletonEnumerator(object val)
            {
                this._value = val;
                _isReset = true;
            }

            public object Current
            {
                get
                {
                    if (_isReset) throw new InvalidOperationException(ErrorMessages.Metadata_List_Enum_Error);
                    return _value;
                }
            }

            public bool MoveNext()
            {
                if (_isReset)
                {
                    _isReset = false;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _isReset = true;
            }
        }

        private static bool MoveNext(IEnumerator[] enumerators)
        {
            for (int i = 0; i < enumerators.Length; i++)
            {
                if (enumerators[i].MoveNext())
                {
                    return true;
                }
                else
                {
                    enumerators[i].Reset();
                    bool moveFlag = enumerators[i].MoveNext();
                    Debug.Assert(moveFlag);
                }
            }
            return false;
        }

        private static IEnumerator GetEnumerator(object arg)
        {
            IEnumerable enumerable = arg as IEnumerable;
            if (enumerable != null && !(arg is string)) return enumerable.GetEnumerator();
            return new SingletonEnumerator(arg);
        }

    }
}
