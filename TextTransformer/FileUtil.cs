using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Regtransf.GUI
{
    public static class FileUtil
    {
        public static void SaveTo(Object value, string filePath)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, value);
            }
        }

        public static T RestoreFrom<T>(string filePath)
        {
            IFormatter formatter = new BinaryFormatter();
            T rval = default(T);
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                rval = (T)formatter.Deserialize(stream);
            }
            return rval;
        }

    }
}
