using OperationsResearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace OperationsResearch.Structures;

public class TransportProblemTable : ICloneable
{
    public List<List<object>> Rows;
    public List<int> Capacity => GetCapacity();
    public List<int> Requests => GetRequests();

    public TransportProblemTable()
    {
        Rows = new();
    }

    private List<int> GetCapacity()
    {
        var list = new List<int>();
        for (var i = 0; i < Rows.Count - 1; i++)
        {
            list.Add((int)Rows[i].Last());
        }
        return list;
    }
    private List<int> GetRequests()
    {
        return Rows[^1].Cast<int>().ToList();
    }

    public object Clone()
    {
        var tpt = new TransportProblemTable();
        foreach (var row in Rows)
        {
            tpt.Rows.Add(row);
        }
        return tpt;
    }

    public string ToLongString()
    {
        var output = "";

        output += "\t";
        var rc = 1;
        foreach (var value in Rows[^1]) { output += $"B{rc}={value}\t"; rc++; }
        for (var i = 0; i < Rows.Count - 1; i++)
        {
            output += "\n";
            output += $"A{i + 1}={Rows[i].Last()}\t";
            for  (var j = 0; j < Rows[i].Count - 1; j++) { output += Rows[i][j] + "\t"; }
        }

        return output;
    }

    public TransportProblemTable Normalized()
    {
        var Values = (TransportProblemTable)Clone();

        // Проверим необходимое и достаточное условие разрешимости задачи
        LogService.Log("Проверим необходимое и достаточное условие разрешимости задачи:");

        var totalcap = Values.Capacity.Sum();
        var totalreq = Values.Requests.Sum();
        LogService.Log($"Запасы: {totalcap}\nПотребности: {totalreq}");

        // Оценим знаения
        if (totalcap == totalreq)
        {
            LogService.Log("Значения равны, следовательно необходимое и достаточное условие разрешимости задачи выполняется");
        }
        else
        {
            var diff = Math.Abs(totalcap - totalreq);
            if (totalcap > totalreq)
            {
                LogService.Log($"Как видно, суммарная потребность груза в пунктах назначения меньше запасов груза на базах. Следовательно, модель исходной транспортной задачи является открытой. Чтобы получить закрытую модель, введем дополнительную (фиктивную) потребность, равной {totalcap} - {totalreq} = {diff}. Тарифы перевозки единицы груза к этому потребителю полагаем равны нулю.\r\nЗанесем исходные данные в распределительную таблицу.");
                for (var i = 0; i < Values.Rows.Count - 1; i++)
                {
                    Values.Rows[i].Insert(Values.Rows[i].Count - 1, 0);
                }
                Values.Rows[^1].Add(diff);
            }
            else
            {
                LogService.Log($"Как видно, суммарная потребность груза в пунктах назначения превышает запасы груза на базах. Следовательно, модель исходной транспортной задачи является открытой. Чтобы получить закрытую модель, введем дополнительную (фиктивную) базу с запасом груза, равным {totalreq} - {totalcap} = {diff}. Тарифы перевозки единицы груза из базы ко всем потребителям полагаем равны нулю.\r\nЗанесем исходные данные в распределительную таблицу.");
                List<object> newRow = new();
                foreach (var _ in Rows[^1]) { newRow.Add(0); }
                newRow.Add(diff);
                Values.Rows.Insert(Values.Rows.Count - 1, newRow);
            }
            LogService.Log(Values.ToLongString());
        }

        return Values;
    }
}
