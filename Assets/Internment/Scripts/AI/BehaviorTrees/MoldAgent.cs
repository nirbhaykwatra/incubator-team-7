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
    private bool _hasPursuedPlayer = false;
    
    private void Awake()
    {
        _characterMovement = GetComponent<CharacterMovement3D>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = FindFirstObjectByType<PlayerController>();
        
        tree = new BehaviorTree();
        Sequence pursuit = new Sequence("Pursuit");
        Leaf idle = new Leaf("Idle", Idle);
        Leaf pursue = new Leaf("Pursue", Pursue);
        Leaf capture = new Leaf("Capture", Capture);
        
        //pursuit.AddChild(idle);
        pursuit.AddChild(pursue);
        pursuit.AddChild(capture);
        tree.AddChild(pursuit);
        
        tree.PrintTree();
    }
    
    private void Update()
    {
        if (treeStatus == Status.Running)
        {
            treeStatus = tree.Process();
        }
    }

    public Status Idle()
    {
        if (_player == null || _navMeshAgent == null || _characterMovement == null) return Status.Failure;
        
        float distance = Vector3.Distance(transform.position, _player.gameObject.transform.position);
        if (distance < 2f)
        {
            return Status.Success;
        }
        
        return Status.Running;
    }

    public Status Capture()
    {
        if (_player == null || _navMeshAgent == null || _characterMovement == null) return Status.Failure;
        _characterMovement.MoveTo(Vector3.zero);
        float distance = Vector3.Distance(transform.position, Vector3.zero);
        if (distance < 0.2f)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    public Status Pursue()
    {
        if (_player == null || _navMeshAgent == null || _characterMovement == null) return Status.Failure;
        float distance = Vector3.Distance(transform.position, _player.gameObject.transform.position);
        Debug.Log(distance);
        _characterMovement.MoveTo(_player.transform.position);

        if (distance < 2f)
        {
            return Status.Success;
        }
        
        return Status.Running;
    }
}
