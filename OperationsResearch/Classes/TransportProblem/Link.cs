namespace OperationsResearch.Classes.TransportProblem;
public class Link
{
    public NodeBase Left;
    public NodeBase Right;
    public int Cost;

    public Link(NodeBase left, NodeBase right, int cost)
    {
        Left = left;
        Right = right;
        Cost = cost;
    }
}
