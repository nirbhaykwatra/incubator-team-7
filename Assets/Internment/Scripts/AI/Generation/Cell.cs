using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [ShowInInspector]
    public float Lifespan { get; set; } = 1f;
    [ShowInInspector]
    public float PropagationRate { get; set; } = 1f;
    [ShowInInspector]
    public bool InfiniteLife { get; set; } = true;
    
    [ShowInInspector]
    [ReadOnly]
    public Vector3Int Coordinates { get; set; }
    
    [ShowInInspector]
    [ReadOnly]
    public bool Infected { get; set; }
    
    private MeshRenderer _renderer;
    private Grid _grid;
    private float _timer;
    private float _lifeSpanTimer;
    private Cell _adjacentTop;
    private Cell _adjacentBottom;
    private Cell _adjacentLeft;
    private Cell _adjacentRight;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _timer = 0;
        _lifeSpanTimer = 0;
    }

    private void Start()
    {
        _grid = GetComponentInParent<Grid>();
    }

    private void Update()
    {
        _renderer.enabled = Infected;
        
        if (Infected && !_grid.AllCellsInfected())
        {
            _timer += Time.deltaTime;

            if (!InfiniteLife)
            {
                _lifeSpanTimer += Time.deltaTime;
                if (_lifeSpanTimer >= Lifespan)
                {
                    _grid.DestroyCell(this);
                }
            }
            if (_timer > PropagationRate)
            {
                Spread();
                _timer = 0.5f;
            }
        }
    }

    [Button]
    public void Spread()
    {
        _adjacentTop = _grid.GetCell(Coordinates.x + 1, Coordinates.y, Coordinates.z);
        _adjacentBottom = _grid.GetCell(Coordinates.x - 1, Coordinates.y, Coordinates.z);
        _adjacentLeft = _grid.GetCell(Coordinates.x, Coordinates.y, Coordinates.z - 1);
        _adjacentRight = _grid.GetCell(Coordinates.x, Coordinates.y, Coordinates.z + 1);
        
        if(_adjacentTop && _adjacentTop.Infected == false) _adjacentTop.Infected = true;
        if(_adjacentBottom && _adjacentBottom.Infected == false) _adjacentBottom.Infected = true;
        if(_adjacentLeft && _adjacentLeft.Infected == false) _adjacentLeft.Infected = true;
        if(_adjacentRight && _adjacentRight.Infected == false) _adjacentRight.Infected = true;
        
        Debug.Log($"Spreading infection!");
    }

    [Button]
    public void Infect()
    {
        Infected = !Infected;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
