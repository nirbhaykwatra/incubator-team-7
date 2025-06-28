using UnityEngine;

public class Sequence : Node
{
    public Sequence(string name)
    {
        Name = name;
    }

    public override Status Process()
    {
        Debug.Log($"Sequence: {Name}");
        Status childStatus = Children[CurrentChild].Process();
        if (childStatus == Status.Failure) return Status.Failure;
        if (childStatus == Status.Running) return Status.Running;

        CurrentChild++;
        if (CurrentChild >= Children.Count)
        {
            CurrentChild = 0;
            //return Status.Success;
        }
        
        return Status.Running;
    }
}
