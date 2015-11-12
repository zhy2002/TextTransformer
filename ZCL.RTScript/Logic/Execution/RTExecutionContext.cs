
using System.Collections.Generic;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Expression;


namespace ZCL.RTScript.Logic.Execution
{
    public class RTExecutionContext
    {
        protected string _srcText;
        private IList<RTExecutionError> _errors = new List<RTExecutionError>();
        private Stack<RTScope> _scopes = new Stack<RTScope>();
        private RTScope _rootScope;
        private RTScope _currentScope;

        public RTExecutionContext(IRTMetadataFactory factory)
        {
            MetadataFactory = factory;
        }

        public string SourceCode
        {
            get
            {
                return _srcText;
            }
        }

        public IRTMetadataFactory MetadataFactory
        {
            get;
            private set;
        }

        //indicate an error has occurred while execution
        public bool HasError
        {
            get
            {
                return _errors.Count != 0;
            }
        }

        public RTVariable DefiningSymbol
        {
            get
            {
                return this.CurrentScope.GetValue(RTSysSymbols.DEFINING_SYMBOL) as RTVariable;
            }

            set
            {
                this.CurrentScope.SetValueLocal(RTSysSymbols.DEFINING_SYMBOL, value);
            }
        }

        public IList<RTExecutionError> Errors
        {
            get
            {
                return _errors;
            }
        }

        public RTScope CurrentScope
        {
            get
            {
                return _currentScope;
            }
        }

        public RTScope RootScope
        {
            get
            {
                return _rootScope;
            }
        }

        public void PushScope(RTScope scope)
        {
            scope.Parent = CurrentScope;
            _scopes.Push(scope);
            _currentScope = scope;

            if (scope.Parent == null)
            {
                _rootScope = scope;
                InitRootScope();
            }
        }

        /// <summary>
        /// should be overriden by descendent
        /// </summary>
        protected virtual void InitRootScope()
        {
        }


        public RTScope PopScope()
        {
            var scope = _scopes.Pop();
            _currentScope = scope.Parent;
            scope.Parent = null;
            return scope;
        }

    }
}
