using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SourceProlongation
{
    public static class MyStatic
    {
        public static string CleanPath(string path)
        {
            return Path.GetInvalidPathChars()
                .Aggregate(path, (current, c) => current.Replace(c.ToString(), String.Empty));
        }

        public static string CleanName(string sh)
        {
            var op = Path.GetInvalidPathChars()
                .Aggregate(sh, (current, c) => current.Replace(c.ToString(), String.Empty));
            op = op.Replace("\\", "-");
            op = op.Replace("/", "-");
            return op;
        }
    }
}
