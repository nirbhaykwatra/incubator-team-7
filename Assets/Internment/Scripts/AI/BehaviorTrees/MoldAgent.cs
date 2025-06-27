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
        if (treeStatus == Status.Running) treeStatus = tree.Process();
    }

    public Status Capture()
    {
        if (_player == null) return Status.Failure;
        if (_characterMovement.StoppingDistance > 0 && Vector3.Distance(_navMeshAgent.destination, transform.position) <
            _characterMovement.StoppingDistance)
        {
            return Status.Success;
        }
        else
        {
            _characterMovement.MoveTo(Vector3.zero);
            return Status.Running;
        }
    }

    public Status Pursue()
    {
        if (_player == null) return Status.Failure;
        if (_characterMovement.StoppingDistance > 0 && Vector3.Distance(_navMeshAgent.destination, transform.position) <
            _characterMovement.StoppingDistance)
        {
            return Status.Success;
        }
        else
        {
            _characterMovement.MoveTo(_player.transform.position);
            return Status.Running;
        }
    }
}
