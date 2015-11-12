

namespace ZCL.RTScript.Logic.Execution
{
    //data conversion between primitives
    //returns null if cannot convert
    public class RTConverter
    {
        public double? ToNumber(object arg)
        {
            if (arg is bool) return (bool)arg ? 1 : 0;
            string val = arg.ToString().Trim();
            double retval;
            if (double.TryParse(val, out retval))
            {
                return retval;
            }

            return double.NaN;
        }

        public bool? ToBoolean(object arg)
        {
            if (arg is bool) return (bool)arg;
            if (arg is double) return (double)arg == 0 ? false : true;
            if (arg is string) return (string)arg == string.Empty ? false : true;
            return null;
        }

        public string ToString(object arg)
        {
            if (arg is string) return arg as string;
            if (arg is double) return arg.ToString();
            if (arg is bool) return (bool)arg ? "true" : "false";
            return null;
        }

        private static RTConverter _converter = new RTConverter();

        public static RTConverter Singleton
        {
            get
            {
                return _converter;
            }
        }
    }
}
