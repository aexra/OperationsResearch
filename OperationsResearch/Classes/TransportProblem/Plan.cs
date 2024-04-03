using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OperationsResearch.Classes.TransportProblem;
public class Plan
{
    public object[][] Mask;
    public List<Vector4> Path;

    public Plan(object[][] mask, List<Vector4> path)
    {
        Mask = mask;
        Path = path;
    }

    public int BasisCount()
    {
        var s = 0;
        foreach (var r in Mask)
        {
            foreach (var v in r)
            {
                if (!(v is char)) s++;
            }
        }
        return s;
    }
}
