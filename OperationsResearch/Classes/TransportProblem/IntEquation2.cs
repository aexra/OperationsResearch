using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsResearch.Classes.TransportProblem;
public class IntEquation2
{
    public int? Left;
    public int? Right;
    public int? Equal;

    public IntEquation2(int? left, int? right, int? equals)
    {
        Left = left;
        Right = right;
        Equal = equals;
    }
    public int? Solve()
    {
        if (Left == null && Right != null && Equal != null)
        {
            return Equal - Right;
        }
        else if (Right == null && Left != null && Equal != null)
        {
            return Equal - Left;
        }
        else if (Equal == null && Left != null && Right != null)
        {
            return Left + Right;
        }
        else
        {
            return null;
        }
    }
}
