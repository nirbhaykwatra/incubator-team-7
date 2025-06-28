using System.Collections.Generic;
using UnityEngine;

public enum Status
{
    Success,
    Failure,
    Running,
}

public class Node
{
    public string Name;
    public Status NodeStatus;
    public List<Node> Children = new List<Node>();
    public int CurrentChild = 0;
    
    public Node() { }
    
    public Node(string name)
    {
        Name = name;
    }

    public virtual Status Process()
    {
        return Children[CurrentChild].Process();
    }
    
    public void AddChild(Node node)
    {
        Children.Add(node);
    }
    
    public string PrintNode()
    {
        string msg = "";
        msg += Name;
        
        foreach(Node node in Children) 
        {
            msg += "\n" + node.PrintNode();
        }
        return msg;
    }
}
