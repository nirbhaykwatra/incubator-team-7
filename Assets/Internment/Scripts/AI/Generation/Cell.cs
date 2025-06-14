using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [ShowInInspector]
    public float Lifespan { get; set; } = 1f;
    [ShowInInspector]
    public float PropagationRate { get; set; } = 0.5f;
    [ShowInInspector]
    public bool InfiniteLife { get; set; } = false;
    
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

        Spread();
        if (Infected)
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
                _timer = 0.5f;
            }
        }
    }

    [Button]
    public void Spread()
    {
        Cell adjacentTop = _grid.GetCell(Coordinates.x + 1, Coordinates.y, Coordinates.z);
        Cell adjacentBottom = _grid.GetCell(Coordinates.x - 1, Coordinates.y, Coordinates.z);
        Cell adjacentLeft = _grid.GetCell(Coordinates.x, Coordinates.y, Coordinates.z - 1);
        Cell adjacentRight = _grid.GetCell(Coordinates.x, Coordinates.y, Coordinates.z + 1);
        
        if(adjacentTop && adjacentTop.Infected) Infected = true;
        if(adjacentBottom && adjacentBottom.Infected) adjacentBottom.Infected = true;
        if(adjacentLeft && adjacentLeft.Infected) adjacentLeft.Infected = true;
        if(adjacentRight && adjacentRight.Infected) adjacentRight.Infected = true;
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
