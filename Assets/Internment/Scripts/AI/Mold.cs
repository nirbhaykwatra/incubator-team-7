using Sirenix.OdinInspector;
using UnityEngine;

public class Mold : MonoBehaviour
{
    public float PropagationSpeed = 0.5f;
    public float PropagationArea = 0.5f;
    
    private Grid _grid;

    private void Awake()
    {
        _grid = FindAnyObjectByType<Grid>();
    }

    private void Start()
    {
        
    }

    [Button]
    public void SpawnMold()
    {
        _grid.GetRandomCell().Infected = true;
    }
}
