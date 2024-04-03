using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OperationsResearch.Classes.TransportProblem;
public class Plan
{
    public int[][] Values;
    public object[][] Mask;
    public List<Vector4> Path;
    public TransportProblem Problem;

    public Plan(TransportProblem problem, object[][] mask, List<Vector4> path)
    {
        Problem = problem;
        Mask = mask;
        Path = path;

        Values = new int[mask.Length][];
        for (var y = 0; y < mask.Length; y++)
        {
            Values[y] = new int[mask[y].Length];
        }
        for (var y = 0; y < mask.Length; y++)
        {
            for (var x = 0; x < mask[y].Length; x++)
            {
                if (mask[y][x] is char)
                {
                    Values[y][x] = (int)path.Find(v => v.X == x && v.Y == y).W;
                }
                else
                {
                    Values[y][x] = (int)mask[y][x];
                }
            }
        }
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
        List<Vector2> path = new();
        path.Add(new(minDelta.X, minDelta.Y));
        var closed = false;
        bool? dir = null;
        while (!closed)
        {
            // Впервые в цикле (ищем первый отрезок)
            if (dir == null)
            {
                // Пробуем найти по вертикали
                for (var y = 0; y < deltas.Length; y++)
                {
                    if (deltas[y][(int)path.First().X] is char)
                    {
                        path.Add(new(y, (int)path.First().X));
                        dir = true;
                    }
                }

                // Пробуем найти по горизонтали
                for (var x = 0; x < deltas[0].Length; x++)
                {
                    if (deltas[(int)path.First().Y][x] is char)
                    {
                        path.Add(new(x, (int)path.First().Y));
                        dir = false;
                    }
                }
            }
            // Нужна горизонталь
            else if (dir.Value)
            {
                for (var x = 0; x < deltas[0].Length; x++)
                {
                    var y = (int)path.Last().Y;
                    if (deltas[y][x] is char)
                    {
                        if (path.Exists(v => v.X == x && v.Y == y)) continue;
                        path.Add(new(x, y));
                        dir = false;
                    }
                }
            }
            // Нужна вертикаль
            else
            {
                for (var y = 0; y < deltas.Length; y++)
                {
                    var x = (int)path.Last().X;
                    if (deltas[y][x] is char)
                    {
                        if (path.Exists(v => v.X == x && v.Y == y)) continue;
                        path.Add(new(x, y));
                        dir = true;
                    }
                }
            }

            if (path.First().X == path.Last().X && path.First().Y == path.Last().Y) break;
        }
        
        // Соберем все негативные перемещения из пути
        List<Vector3> negative = new();
        for (var i = 1; i < path.Count; i += 2)
        {
            negative.Add(new(path[i].X, path[i].Y, Problem.GetCost((int)path[i].Y, (int)path[i].X)));
        }

        // Минимальное из них
        var minCost = negative.Select(x => (int)x.Z).Min();

        // Пройдем по пути и изменим цену значений в маске

    }
}
