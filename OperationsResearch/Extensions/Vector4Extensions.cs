using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OperationsResearch.Extensions;

public static class Vector4Extensions
{
    public static string ToLongString(this Vector4 vec)
    {
        return $"{vec.X}, {vec.Y}, {vec.Z}, {vec.W}";
    }
}
