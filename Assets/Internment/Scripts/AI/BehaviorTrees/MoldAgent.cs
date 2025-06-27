using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MoldAgent : MonoBehaviour
{
    private BehaviorTree tree;
    
    private CharacterMovement3D _characterMovement;
    private NavMeshAgent _navMeshAgent;
    private PlayerController _player;
    
    private Status treeStatus = Status.Running;
    
    private void Awake()
    {
        _characterMovement = GetComponent<CharacterMovement3D>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = FindFirstObjectByType<PlayerController>();
        
        tree = new BehaviorTree();
        Sequence pursuit = new Sequence("Pursuit");
        Leaf pursue = new Leaf("Pursue", Pursue);
        Leaf capture = new Leaf("Capture", Capture);
        
        pursuit.AddChild(pursue);
        pursuit.AddChild(capture);
        tree.AddChild(pursuit);
        
        tree.PrintTree();
    }
    
    private void Update()
    {
        Debug.Log($"Node int: {tree.CurrentChild}");
        Debug.Log($"Tree status: {treeStatus}");
        if (treeStatus == Status.Running)
        {
            treeStatus = tree.Process();
        }
    }

    public Status Capture()
    {
        if (_player == null || _navMeshAgent == null || _characterMovement == null) return Status.Failure;
        _characterMovement.MoveTo(Vector3.zero);
        float distance = Vector3.Distance(_navMeshAgent.destination, Vector3.zero);
        if (distance < 0.2f)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    public Status Pursue()
    {
        if (_player == null || _navMeshAgent == null || _characterMovement == null) return Status.Failure;
        float distance = Vector3.Distance(_navMeshAgent.destination, _player.transform.position);
        _characterMovement.MoveTo(_player.transform.position);

        if (distance < 2f)
        {
            return Status.Success;
        }
        
        return Status.Running;
    }
}
