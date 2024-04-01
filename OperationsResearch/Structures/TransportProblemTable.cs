using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsResearch.Structures;

internal class TransportProblemTable : ICloneable
{
    public List<List<object>> Rows;
    
    public TransportProblemTable()
    {
        Rows = new();
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
            output += $"A{i + 1}={Rows[i][0]}\t";
            for  (var j = 1; j < Rows[i].Count; j++) { output += Rows[i][j] + "\t"; }
        }

        return output;
    }
}
