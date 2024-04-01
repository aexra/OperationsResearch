using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsResearch.Structures;

internal class Table
{
    public List<string> Headers;
    public List<string> Names;
    public List<List<object>> Rows;
    
    public Table()
    {
        Headers = new();
        Names = new();
        Rows = new();
    }

    public string ToLongString()
    {
        var output = "";

        var hasHeader = Headers.Count > 0;
        var hasHeaderColumn = Names.Count > 0;

        if (hasHeader)
        {
            if (hasHeaderColumn) output += "\t";
            foreach (var header in Headers) { output += header + "\t"; }
            output += "\n";
        }

        var rowCounter = 0;
        foreach (var row in Rows)
        {
            if (rowCounter > 0) output += "\n";
            if (hasHeaderColumn) { output += Names[rowCounter] + "\t"; }
            foreach (var value in row) { output += value + "\t"; }
            rowCounter++;
        }

        return output;
    }
}
