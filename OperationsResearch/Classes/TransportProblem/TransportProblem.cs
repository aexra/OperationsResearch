using System;
using System.Collections.Generic;
using System.Numerics;
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
    public Vector4 GetInitialPlanMask()
    {
        var vec = new Vector4();



        return vec;
    }

    // NODES MANIPULATION METHODS
    public void CreateMutualLink(int p, int c, int cost)
    {
        Providers[p].Connect(Consumers[c], cost);
        Consumers[c].Connect(Consumers[p], cost);
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
