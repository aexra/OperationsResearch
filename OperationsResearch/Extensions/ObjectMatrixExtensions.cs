using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsResearch.Extensions;
public static class ObjectMatrixExtensions
{
    public static string ToLongString(this object[][] a)
    {
        var s = "";

        for (var i = 0; i < a.Length; i++)
        {
            if (i != 0) s += "\n";
            s += "[ ";
            s += string.Join(",\t", a[i]);
            s += "]";
        }

        return s;
    }
}
