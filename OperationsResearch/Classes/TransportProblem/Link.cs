using System;

namespace OperationsResearch.Classes.TransportProblem;
public class Link : IEquatable<Link>
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

    // INTERFACE IMPLEMENTATION
    public bool Equals(Link other)
    {
        return Left == other.Left && Right == other.Right;
    }
}
