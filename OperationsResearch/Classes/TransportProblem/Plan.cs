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
    public TransportProblem Problem;

    public Plan(TransportProblem problem, object[][] mask, List<Vector4> path)
    {
        Problem = problem;
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
    public void GetUVPotentials(out int[] us, out int[] vs)
    {
        var us_t = new int?[Mask.Length];
        var vs_t = new int?[Mask[0].Length];

        us_t[0] = 0;
        var solved = false;
        while (!solved)
        {
            solved = true;
            foreach (var cell in Path)
            {
                if (us_t[(int)cell.Y] != null && vs_t[(int)cell.X] == null)
                {
                    vs_t[(int)cell.X] = new IntEquation2(us_t[(int)cell.Y], vs_t[(int)cell.X], (int)cell.W).Solve();
                    solved = false;
                }
                else if (us_t[(int)cell.Y] == null && vs_t[(int)cell.X] != null)
                {
                    us_t[(int)cell.Y] = new IntEquation2(us_t[(int)cell.Y], vs_t[(int)cell.X], (int)cell.W).Solve();
                    solved = false;
                }
            }
        }

        us = us_t.Cast<int>().ToArray();
        vs = vs_t.Cast<int>().ToArray();
    }
    public object[][] GetIndirectCosts()
    {
        GetUVPotentials(out var us, out var vs);

        var mask = new object[us.Length][];
        for (var i = 0; i < us.Length; i++)
        {
            mask[i] = new object[vs.Length];
        }

        // Заполним Х все базисные точки
        foreach (var cell in Path)
        {
            mask[(int)cell.Y][(int)cell.X] = 'x';
        }

        // Рассчитаем Dij для каждой небазисной точки
        for (var i = 0; i < mask.Length; i++)
        {
            for (var j = 0; j < mask[i].Length; j++)
            {
                if (!(mask[i][j] is char))
                {
                    mask[i][j] = Problem.GetCost(i, j) - (us[i] + vs[j]);
                }
            }
        }

        return mask;
    }
    public bool IsOptimal()
    {
        List<int> deltas = new();
        GetIndirectCosts().ToList().ForEach(mass => mass.Where(x => x is int).ToList().ForEach(x => deltas.Add((int)x)));
        return deltas.Exists(x => x < 0);
    }
    public void Improve()
    {
        var depth = 1000;
        while (depth > 0 && !IsOptimal())
        {
            // Создаю цикл пересчета и меняю таблицу
            Cycle();

            // 
            depth--;
        }
    }
    public void Cycle()
    {
        // Находим минимальную дельта оценку и ее координату
        var deltas = GetIndirectCosts();
        Vector3 minDelta = new(500, 500, 500);
        for (var i = 0; i < deltas.Length; i++)
        {
            for (var j = 0; j < deltas[i].Length; j++)
            {
                if (deltas[i][j] is int && (int)deltas[i][j] < minDelta.Z)
                {
                    minDelta = new(j, i, (int)deltas[i][j]);
                }
            }
        }

        // Начинаем цикл из нее
        List<Vector3> negative = new();
        List<Vector2> path = new();
        path.Add(new(minDelta.X, minDelta.Y));
        var closed = false;
        while (!closed)
        {

        }
    }
}
