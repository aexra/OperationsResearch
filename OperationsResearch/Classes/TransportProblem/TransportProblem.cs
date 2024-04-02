using OperationsResearch.Enums;
using OperationsResearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;

namespace OperationsResearch.Classes.TransportProblem;
public class TransportProblem : ICloneable
{
    public List<Provider> Providers;
    public List<Consumer> Consumers;

    public TransportProblem(int[] providers, int[] consumers, int[][] travelCosts)
    {
        // Создаем пустые списки поставщиков и потребителей
        Providers = new();
        Consumers = new();

        // Заполняем эти списки
        var counter_p = 1;
        var counter_c = 1;
        foreach (var provider in providers)
        {
            Providers.Add(new($"A{counter_p}", provider));
            counter_p++;
        }
        foreach (var consumer in consumers)
        {
            Consumers.Add(new($"B{counter_c}", consumer));
            counter_c++;
        }
    
        // Соединяем их с весами переходов
        for (int i_p = 0; i_p < travelCosts.Length; i_p++)
        {
            for (int i_c = 0; i_c < travelCosts[i_p].Length; i_c++)
            {
                CreateMutualLink(i_p, i_c, travelCosts[i_p][i_c]);
            }
        }
    }

    // PROBLEM METHODS
    public object Solve()
    {
        return new object();
    }
    public TransportProblemNormalizationResult Normalize()
    {
        var capacity = Providers.Select(x => x.Cost).Sum();
        var requests = Consumers.Select(x => x.Cost).Sum();

        if (capacity == requests) return TransportProblemNormalizationResult.Match;
        else if (capacity > requests)
        {
            CreateConsumer(capacity - requests);
            return TransportProblemNormalizationResult.Excess;
        }
        else
        {
            CreateProvider(requests - capacity);
            return TransportProblemNormalizationResult.Deficiency;
        }
    }
    public Vector4 GetInitialPlanMask()
    {
        var path = new Vector4();

        var x = 0;
        var y = 0;

        object[][] planMask = new object[Providers.Count][];
        for (var i_p = 0; i_p < Providers.Count; i_p++)
        {
            planMask[i_p] = Providers[i_p].Links.Select(x => x.Cost).Cast<object>().ToArray();
        }

        //while (true)
        //{
        //    LogService.Log($"Рассматриваемая точка: ({y}, {x})");
        //    if (Values.Rows[y][x] is char)
        //    {
        //        y++;
        //        if (y >= Values.Rows.Count - 1) break;
        //        continue;
        //    }
        //    var min = Math.Min(Values.Capacity[y], Values.Requests[x]);
        //    LogService.Log($"Минимальное: {min}");

        //    Values.Rows[y][^1] = Values.Capacity[y] - min;
        //    Values.Rows[^1][x] = Values.Requests[x] - min;

        //    // х всё что справа
        //    if ((int)Values.Rows[y][^1] == 0)
        //    {
        //        for (var i = x + 1; i < Values.Requests.Count; i++)
        //        {
        //            Values.Rows[y][i] = 'x';
        //        }
        //    }
        //    // х всё что снизу
        //    else if ((int)Values.Rows[^1][x] == 0)
        //    {
        //        for (var i = y + 1; i < Values.Capacity.Count; i++)
        //        {
        //            Values.Rows[i][x] = 'x';
        //        }
        //    }

        //    LogService.Log("Результат:\n" + Values.ToLongString());

        //    path.Add(new(x, y, min, (int)Values.Rows[y][x]));


        //    if ((int)Values.Rows[y][^1] == 0) { y++; }
        //    else if ((int)Values.Rows[^1][x] == 0) { x++; }

        //    if (x >= Values.Rows[^1].Count) { x = 0; y++; }
        //    if (y >= Values.Rows.Count - 1) { y--; }
        //    if (x == Values.Rows[^1].Count - 1 && y == Values.Rows.Count - 2) break;
        //}

        return path;
    }

    // NODES MANIPULATION METHODS
    public void CreateMutualLink(int p, int c, int cost)
    {
        Providers[p].Connect(Consumers[c], cost);
        Consumers[c].Connect(Consumers[p], cost);
    }
    public Consumer CreateProvider(int cost, int[] travelCosts = null)
    {
        Providers.Add(new($"A{Providers.Count + 1}", cost));
        if (travelCosts == null)
        {
            for (var i_c = 0; i_c < Consumers.Count; i_c++)
            {
                CreateMutualLink(i_c, Providers.Count - 1, 0);
            }
        }
        else
        {
            for (var i_c = 0; i_c < Consumers.Count; i_c++)
            {
                CreateMutualLink(i_c, Providers.Count - 1, travelCosts[i_c]);
            }
        }
        return Consumers.Last();
    }
    public Consumer CreateConsumer(int cost, int[] travelCosts = null)
    {
        Consumers.Add(new($"B{Consumers.Count + 1}", cost));
        if (travelCosts == null)
        {
            for (var i_p = 0; i_p < Providers.Count; i_p++)
            {
                CreateMutualLink(i_p, Consumers.Count - 1, 0);
            }
        }
        else
        {
            for (var i_p = 0; i_p < Providers.Count; i_p++)
            {
                CreateMutualLink(i_p, Consumers.Count - 1, travelCosts[i_p]);
            }
        }
        return Consumers.Last();
    }

    // HELPER METHODS
    public string GetTableString()
    {
        var s = "";

        foreach (var consumer in Consumers)
        {
            s += $"\t{consumer.Name}={consumer.Cost}";
        }
        foreach (var provider in Providers)
        {
            s += $"\n{provider.Name}={provider.Cost}";
            foreach (var consumer in Consumers)
            {
                s += $"\t{provider.Links.Find(x => x.Right == consumer).Cost}";
            }
        }

        return s;
    }
    public int GetCost(NodeBase n1, NodeBase n2)
    {
        return n1.Links.Find(x => x.Right == n2).Cost;
    }

    // INTERFACE IMPLEMENTATION
    public object Clone()
    {
        int[] ps = new int[Providers.Count];
        int[] cs = new int[Consumers.Count];
        int[][] costs = new int[Providers.Count][];

        for (var i_p = 0; i_p < Providers.Count; i_p++)
        {
            ps[i_p] = Providers[i_p].Cost;
        }
        for (var i_c = 0; i_c < Consumers.Count; i_c++) 
        {
            cs[i_c] = Consumers[i_c].Cost;
        }

        for (var i_p = 0; i_p < Providers.Count; i_p++)
        {
            var costs_p = new int[Consumers.Count];
            for (var i_c = 0; i_c < Consumers.Count; i_c++)
            {
                costs_p[i_c] = Providers[i_p].Links.Find(x => x.Right == Consumers[i_c]).Cost;
            }
            costs[i_p] = costs_p;
        }

        return new TransportProblem(ps, cs, costs);
    }
}
