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
            for (var j = 0; j < a[i].Length; j++)
            {
                s += a[i][j].ToString() + ", ";
            }
            s += "]";
        }

        return s;
    }
}
