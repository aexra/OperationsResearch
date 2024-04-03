using ABI.System.Collections.Generic;
using OperationsResearch.Extensions;
using OperationsResearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace OperationsResearch.Classes.TransportProblem;
public class Plan
{
    public string Mask => GetPlanString();
    public List<Vector4> Path;
    public TransportProblem Problem;

    public Plan(TransportProblem problem, List<Vector4> path)
    {
        Problem = problem;
        Path = path;
    }

    public string GetPlanString()
    {
        var s = "";

        var counter = 0;
        for (var i_p = 0; i_p < Problem.Providers.Count; i_p++)
        {
            s += $"{(counter != 0? "\n" : "")}[";
            for (var i_c = 0; i_c < Problem.Consumers.Count; i_c++)
            {
                if (Path.Exists(v => v.X == i_c && v.Y == i_p))
                {
                    var cell = Path.Find(v => v.X == i_c && v.Y == i_p);
                    s += $"{cell.W}|{cell.Z},\t";
                }
                else
                {
                    s += $"{Problem.GetCost(i_p, i_c)},\t";
                }
            }
            s += "]";
            counter++;
        }

        return s;
    }
    public void GetUVPotentials(out int[] us, out int[] vs)
    {
        var us_t = new int?[Problem.Providers.Count];
        var vs_t = new int?[Problem.Consumers.Count];

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
        return !deltas.Exists(x => x < 0);
    }
    public bool Improve(int depth = 100)
    {
        while (depth > 0 && !IsOptimal())
        {
            // Создаю цикл пересчета и меняю таблицу
            if (!Cycle()) return false;

            // 
            depth--;
        }
        return true;
    }
    private bool Cycle()
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
        if (GetCycle(new(minDelta.X, minDelta.Y), null, null, out var closed_path, 10))
        {
            closed_path.Insert(0, new(minDelta.X, minDelta.Y));
            closed_path.Add(new(minDelta.X, minDelta.Y));
            LogService.Log(string.Join(" -> ", closed_path.Select(v => $"({v.Y},{v.X})")));
            closed_path.RemoveAt(closed_path.Count - 1);

            List<Vector2> negative = new();
            for (var i = 1; i < closed_path.Count; i += 2)
            {
                negative.Add(closed_path[i]);
            }
            List<Vector2> positive = new();
            for (var i = 2; i < closed_path.Count; i += 2)
            {
                positive.Add(closed_path[i]);
            }

            //LogService.Error(negative.Count);
            //LogService.Error(positive.Count);

            // Получим "минимальный" базис среди тех что будут подвергнуты уменьшению
            //var minn = negative.Select(v => Path.Find(p => p.X == v.X && p.Y == v.Y).Z).Min();
            Vector3 minn = new(-1, -1, Path.Find(v => v.X == negative.First().X && v.Y == negative.First().Y).Z);
            foreach (var neg in negative)
            {
                foreach (var v in Path)
                {
                    if (v.X == neg.X && v.Y == neg.Y)
                    {
                        if (v.Z < minn.Z) minn = new(v.X, v.Y, v.Z); 
                        break;
                    }
                }
            }

            // Уберем его из пути и из ... вообще
            Path.Remove(Path.Find(v => v.X == minn.X && v.Y == minn.Y));
            negative.Remove(negative.Find(v => v.X == minn.X && v.Y == minn.Y));

            // Добавим новый в точке minDelta
            Path.Add(new(minDelta.X, minDelta.Y, minn.Z, Problem.GetCost((int)minDelta.Y, (int)minDelta.X)));

            // Изменим остальные значения
            foreach (var neg in negative)
            {
                for (var i = 0; i < Path.Count; i++)
                {
                    if (Path[i].X == neg.X && Path[i].Y == neg.Y)
                    {
                        Path[i] = new(Path[i].X, Path[i].Y, Path[i].Z - minn.Z, Path[i].W);
                    }
                }
            }
            foreach (var pos in positive)
            {
                for (var i = 0; i < Path.Count; i++)
                {
                    if (Path[i].X == pos.X && Path[i].Y == pos.Y)
                    {
                        Path[i] = new(Path[i].X, Path[i].Y, Path[i].Z + minn.Z, Path[i].W);
                    }
                }
            }

            // Вернем true, что значит что мы внесли изменения в базис
            return true;
        }
        else
        {
            LogService.Error("Цикл не найден");
            return false;
        }
    }
    private bool GetCycle(Vector2 start, bool? direction, List<Vector2> path, out List<Vector2> closed_path, int depth)
    {
        if (path == null)
        {            
            // Пробуем найти точки по горизонтали (true) и вертикали (false)
            foreach (var v in Path)
            {
                // |
                if ((int)v.X == (int)start.X)
                {
                    var n_path = new List<Vector2> { new(v.X, v.Y) };
                    //LogService.Log($"Сделал начальный переход из ({start.Y},{start.X}) в ({v.Y},{v.X})");
                    if (GetCycle(start, true, n_path, out closed_path, depth - 1)) return true;
                }
                // -
                if ((int)v.Y == (int)start.Y)
                {
                    var n_path = new List<Vector2> { new(v.X, v.Y) };
                    //LogService.Log($"Сделал начальный переход из ({start.Y},{start.X}) в ({v.Y},{v.X})");
                    if (GetCycle(start, false, n_path, out closed_path, depth - 1)) return true;
                }
            }

            // Если мы никуда не смогли отсюда даже пойти, сразу вернем false
            closed_path = null;
            return false;
        }
        else
        {
            // Получим предудшую ячейку для удобства
            var prev = path.Last();

            // Нашли конец цикла?
            if (path.Count > 2 && ((int)prev.X == (int)start.X || (int)prev.Y == (int)start.Y))
            {
                //LogService.Error("gotcha");
                closed_path = path;
                return true;
            }

            // Проверяем глубину поиска
            if (depth <= 0)
            {
                closed_path = null;
                return false;
            }

            // Ищем в горизонтали
            if (direction.Value)
            {
                foreach (var v in Path)
                { 
                    if ((int)v.Y == (int)prev.Y)
                    {
                        if (path.Exists(vec => (int)vec.X == (int)v.X && (int)vec.Y == (int)v.Y)) { continue; }
                        var n_path = new List<Vector2>();
                        foreach (var vec in path) { n_path.Add(vec); }
                        n_path.Add(new(v.X, v.Y));
                        //LogService.Log($"Сделал переход из ({prev.Y},{prev.X}) в ({v.Y},{v.X})");
                        if (GetCycle(start, false, n_path, out closed_path, depth - 1)) return true;
                    }
                }
            }
            // Ищем в вертикали
            else
            {
                foreach (var v in Path)
                {
                    if ((int)v.X == (int)prev.X)
                    {
                        if (path.Exists(vec => (int)vec.X == (int)v.X && (int)vec.Y == (int)v.Y)) { continue; }
                        var n_path = new List<Vector2>();
                        foreach (var vec in path) { n_path.Add(vec); }
                        n_path.Add(new(v.X, v.Y));
                        //LogService.Log($"Сделал переход из ({prev.Y},{prev.X}) в ({v.Y},{v.X})");
                        if (GetCycle(start, true, n_path, out closed_path, depth - 1)) return true;
                    }
                }
            }

            closed_path = null;
            return false;
        }
    }
    public int GetTargetValue()
    {
        return Path.Select(x => (int)x.Z * (int)x.W).Sum();
    }
}
