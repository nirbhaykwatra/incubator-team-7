using Sirenix.OdinInspector;
using UnityEngine;

public class Mold : MonoBehaviour
{
    public float PropagationSpeed = 0.5f;
    public float PropagationArea = 0.5f;
    
    private AIGrid aiGrid;

    private void Awake()
    {
        aiGrid = FindAnyObjectByType<AIGrid>();
    }

    private void Start()
    {
        
    }

    [Button]
    public void SpawnMold()
    {
        aiGrid.GetRandomCell().Infected = true;
    }
}
