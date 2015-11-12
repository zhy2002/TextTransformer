using System;
using System.Collections.Generic;

using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Expression;


namespace ZCL.RTScript.Logic.Execution
{
    /// <summary>
    /// scopes can form a chain (linked list).
    /// </summary>
    public class RTScope : IRTExpression
    {
        internal RTScope(RTFunction creator)
        {
            this.Creator = creator;
        }

        private RTScope() { }

        private IDictionary<string, object> _slots;

        /// <summary>
        /// Storage object for the arguments and variables in this scope.
        /// </summary>
        private IDictionary<string, object> Slots
        {
            get
            {
                if (_slots == null)
                {
                    _slots = new SortedDictionary<string, object>();
                }
                return _slots;
            }
        }

        /// <summary>
        /// The parent scope.
        /// is null if the scope is not on stack.
        /// </summary>
        public RTScope Parent
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Function that contains this scope.
        /// This is null if the scope is created by FunctionList and thus is a root scope.
        /// </summary>
        public IRTFunction Creator
        {
            get;
            private set;
        }

        /// <summary>
        /// cann only be set in AddArg function
        /// </summary>
        private int _argCount;

        public int ArgCount
        {
            get
            {
                if (_argCount > 0)
                    return _argCount;
                if (Creator != null) return Creator.Args.Count;
                throw new InvalidOperationException(ErrorMessages.Scope_ArgCount_Error);//bug
            }
        }

        public string FunctionName
        {
            get
            {
                if (Creator == null) return null;
                return Creator.Name;
            }
        }


        /// <summary>
        /// get the first scope in the chain that contains varname.
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        private RTScope GetScope(string varname)
        {
            if (_slots != null)
            {
                if (Slots.ContainsKey(varname)) return this;
            }

            if (Parent != null)
            {
                return Parent.GetScope(varname);
            }

            return null;
        }

        #region get and set variable

        private bool _executed;

        //get the value of varname in scope chain
        public object GetValue(string varname)
        {
            var scope = GetScope(varname);
            if (scope == null) return RTVoid.Singleton;
            return scope.Slots[varname];
        }

        /// <summary>
        /// return true if a new slot is created locally
        /// else an existing slot in the scope chain is set
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public bool SetValue(string varname, object value)
        {
            if (_executed) throw new InvalidOperationException();
            var scope = GetScope(varname);
            if (scope != null)
            {
                scope.Slots[varname] = value;
                return false;
            }

            Slots[varname] = value;
            return true;
        }

        /// <summary>
        /// Get the value of <paramref name="varname"/> from this scope object.
        /// </summary>
        /// <param name="varname"></param>
        /// <returns>Returns VOID if varname is not found in this scope object.</returns>
        public object GetValueLocal(string varname)
        {
            if (_slots == null || !Slots.ContainsKey(varname)) return RTVoid.Singleton;

            return Slots[varname];
        }

        /// <summary>
        /// Set the value of<paramref name="varname"/> in this scope object.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        public void SetValueLocal(string varname, object value)
        {
            if (_executed) throw new InvalidOperationException();
            Slots[varname] = value;
        }

        #endregion

        #region get and set argument

        /// <summary>
        /// A symbol that is not valid as the start symbol of an identifier
        /// </summary>
        private const string ARG_PREFIX = "&";

        /// <summary>
        /// Arguments can be accessed by index
        /// Local variables are named with an identifier
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public int AddArg(object arg)
        {
            if (_executed) throw new InvalidOperationException();
            SetValueLocal(ARG_PREFIX + _argCount.ToString(), arg);
            return _argCount++;
        }

        public object GetArg(int index)
        {
            if (index < 0) return RTVoid.Singleton;

            if (index < _argCount)
                return GetValueLocal(ARG_PREFIX + index.ToString());

            if (index < Creator.Args.Count)
                return Creator.Args[index];

            return RTVoid.Singleton;
        }

        public bool RemoveArg(int index)
        {
            if (_executed) throw new InvalidOperationException();
            if (index >= _argCount || index < 0) return false;
            _argCount--;
            for (int i = index; i < _argCount; i++)
            {
                _slots[ARG_PREFIX + index.ToString()] = _slots[ARG_PREFIX + (index + 1).ToString()];
            }
            _slots.Remove(ARG_PREFIX + _argCount.ToString());
            return true;
        }

        #endregion

        public object Execute(RTExecutionContext context)
        {
            if (Creator == null)
                throw new InvalidOperationException(ErrorMessages.Scope_Execute_Error);

            RTScope exeScope = _executed ? Duplicate() : this;
            context.PushScope(exeScope);
            var retval = Creator.Metadata.Execute(context);
            context.PopScope();
            exeScope._executed = true;

            return retval;
        }

        /// <summary>
        /// Get all parameter variables needed to call this scope object.
        /// </summary>
        /// <param name="vars"></param>
        public void GetAllParameters(IList<RTVariable> vars)
        {
            for (int i = 0; i < this.ArgCount; i++)
            {
                var arg = this.GetArg(i);
                RTVariable var = arg as RTVariable;
                if (var != null && !vars.Contains(var))
                {
                    vars.Add(var);
                }
                else
                {
                    RTScope subScope = arg as RTScope;
                    if (subScope != null)
                    {
                        subScope.GetAllParameters(vars);
                    }
                }
            }
        }

        /// <summary>
        /// Deep copy this instance.
        /// </summary>
        /// <returns>A clone.</returns>
        private RTScope Duplicate()
        {
            RTScope scope = new RTScope();
            scope.Creator = this.Creator;
            scope._argCount = this._argCount;

            if (_slots != null)
                foreach (var pair in this._slots)
                {
                    scope.SetValueLocal(pair.Key, pair.Value);
                }
            return scope;
        }
    }

}
