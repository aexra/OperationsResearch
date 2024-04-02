using System.Collections.Generic;

namespace OperationsResearch.Classes.TransportProblem;
public abstract class NodeBase
{
    public List<Link> Links;
    public string Name;
    public int Cost;

    public NodeBase(string name, int weight)
    {
        Name = name;
        Cost = weight;

        Links = new();
    }

    public Link Connect(NodeBase node, int cost)
    {
        var nn = new Link(this, node, cost);
        Links.Add(nn);
        return nn;
    }
}
