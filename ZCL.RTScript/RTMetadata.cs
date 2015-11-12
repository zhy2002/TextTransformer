using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript
{
    /// <summary>
    /// The metadata of a function. 
    /// </summary>
    public abstract class RTMetadata
    {
        /// <summary>
        /// the name of the function
        /// </summary>
        public abstract string FunctionName
        {
            get;
        }

        /// <summary>
        /// An optional description of the function.
        /// </summary>
        public virtual string Description
        {
            get;
            protected set;
        }

        /// <summary>
        /// Determines if the return value is meaningful
        /// </summary>
        public virtual bool HasReturnValue
        {
            get;
            protected set;
        }

        /// <summary>
        /// If you can call def and set system function in this function.
        /// </summary>
        public virtual bool CanDefineVariable
        {
            get;
            protected set;
        }

        protected RTMetadata()
        {
            this.CanDefineVariable = false;
            this.HasReturnValue = true;
        }

        /// <summary>
        /// will not modify the scope object.
        /// execute against the current scope.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract object Execute(RTExecutionContext context);

        /// <summary>
        /// Evaluate the ith arg in the function.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="i"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual object EvaluateArg(int i, RTExecutionContext context)
        {
            var scope = context.CurrentScope;
            object arg = scope.GetArg(i);
            object evaled;
            var expr = arg as IRTExpression;
            if (expr != null)
            {
                evaled = expr.Execute(context);
            }
            else
            {
                evaled = arg;
            }

            RTScope evaledScope = evaled as RTScope;
            if (arg is RTVariable && evaledScope != null)
            {
                evaled = evaledScope.Execute(context); //try to reduct scope referenced by var
            }

            return evaled;
        }

        public virtual void NormalizeArgs(IList<object> args)
        { }

    }
}
