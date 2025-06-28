using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree()
    {
        Name = "Behavior Tree";
    }

    public BehaviorTree(string name)
    {
        Name = name;
    }

    public override Status Process()
    {
        return Children[CurrentChild].Process();
    }

    public void PrintTree() 
    {
        Debug.Log(PrintNode());
    }
}
