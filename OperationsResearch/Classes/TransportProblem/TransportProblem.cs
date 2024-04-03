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
    public List<Vector4> GetInitialPlanMask(out object[][] mask)
    {
        var path = new List<Vector4>();

        var x = 0;
        var y = 0;

        // То что я буду редактировать
        mask = new object[Providers.Count][];
        var capacity = Providers.Select(x => x.Cost).ToArray();
        var requests = Consumers.Select(x => x.Cost).ToArray();

        // Заполняю plan исходными значениями
        for (var i_p = 0; i_p < Providers.Count; i_p++)
        {
            mask[i_p] = Providers[i_p].Links.Select(x => x.Cost).Cast<object>().ToArray();
        }

        while (true)
        {
            // Если там Х, то смещаемся вниз
            if (mask[y][x] is char)
            {
                y++;
                if (y >= mask.Length - 1) break;
                continue;
            }

            // Минимальное среди потребителя и провайдера
            var min = Math.Min(capacity[y], requests[x]);

            // Изменяем величины потребителей и провайдеров
            capacity[y] -= min;
            requests[x] -= min;

            // х всё что справа
            if (capacity[y] == 0)
            {
                for (var i = x + 1; i < requests.Length; i++)
                {
                    mask[y][i] = 'x';
                }
            }
            // х всё что снизу
            else if (requests[x] == 0)
            {
                for (var i = y + 1; i < capacity.Length; i++)
                {
                    mask[i][x] = 'x';
                }
            }

            // Добавляем текущую точку в список вершин пути
            path.Add(new(x, y, min, (int)mask[y][x]));

            // Перемещаем курсор
            if (requests[x] == 0) { x++; }
            else if (capacity[y] == 0) { y++; }

            // Какие-то странные условия выхода мне лень проверять что из этого не нужно
            if (x >= requests.Length) { x = 0; y++; }
            //if (y >= capacity.Length) { y--; }
            if (x == requests.Length - 1 && y == capacity.Length - 1) break;
        }

        return path;
    }
    public int GetInitialTargetValue()
    {
        return GetInitialPlanMask(out _).Select(x => (int)x.Z * (int)x.W).Sum();
    }

    // NODES MANIPULATION METHODS
    public void CreateMutualLink(int p, int c, int cost)
    {
        Providers[p].Connect(Consumers[c], cost);
        Consumers[c].Connect(Providers[p], cost);
    }
    public Consumer CreateProvider(int cost, int[] travelCosts = null)
    {
        Providers.Add(new($"A{Providers.Count + 1}", cost));
        if (travelCosts == null)
        {
            for (var i_c = 0; i_c < Consumers.Count; i_c++)
            {
                CreateMutualLink(Providers.Count - 1, i_c, 0);
            }
        }
        else
        {
            for (var i_c = 0; i_c < Consumers.Count; i_c++)
            {
                CreateMutualLink(Providers.Count - 1, i_c, travelCosts[i_c]);
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
