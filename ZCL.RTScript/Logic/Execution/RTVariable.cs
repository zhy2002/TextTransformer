
using ZCL.RTScript.AbstractionLayer;

namespace ZCL.RTScript.Logic.Execution
{
    /// <summary>
    /// A thin wrapper of string to tell var name from string literal.
    /// </summary>
    public class RTVariable : IRTExpression
    {
        public string Name
        {
            get;
            private set;
        }

        public RTVariable(string name)
        {
            this.Name = name;
        }

        public override bool Equals(object obj)
        {
            RTVariable var = obj as RTVariable;
            if (var != null)
            {
                if (this.Name == var.Name) return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Resolve the value of this variable, which can be this variable itself, another variable, data value or void. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public object Execute(RTExecutionContext context)
        {
            if (context.CurrentScope == null) return this;
            var variable = this;
            object val;
            while (true)
            {
                val = context.CurrentScope.GetValue(variable.Name);
                if (val is RTVoid) return variable;
                variable = val as RTVariable;
                if (variable == null) break;
                else if (variable.Name == this.Name)
                {
                    return this;
                }
            }

            return val;
        }


    }
}
