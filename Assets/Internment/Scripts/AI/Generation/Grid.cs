using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    [FoldoutGroup("Dimensions")]
    [SerializeField] private Vector3Int _gridSize;
    
    [FoldoutGroup("Templates")]
    public GameObject CellPrefab;
    
    private List<Cell> _cells;

    private void Awake()
    {
        _cells = new List<Cell>();
        for (int x = 0; x < _gridSize.x / CellPrefab.transform.localScale.x; x++)
        {
            //for (int y = 0; y < _gridSize.y / CellPrefab.transform.localScale.y; y++)
            //{
            for (int z = 0; z < _gridSize.z / CellPrefab.transform.localScale.z; z++)
            {
                Vector3 position = new Vector3(x * CellPrefab.transform.localScale.x, 0, z * CellPrefab.transform.localScale.z);
                GameObject cellObject = Instantiate(CellPrefab, position, Quaternion.identity);
                cellObject.transform.SetParent(this.transform);
                cellObject.GetComponent<Cell>().Coordinates = new Vector3Int(x, 0, z);
                cellObject.gameObject.name = $"{x}, 0, {z}";
                _cells.Add(cellObject.GetComponent<Cell>());
            }
            //}
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
    
    [Button]
    public void InfectRandomCell()
    {
        GetRandomCell().Infected = true;
    }

    public Cell GetCell(Vector3Int coordinates)
    {
        if (_cells.Exists(cell => cell.Coordinates == coordinates))
        {
            return _cells.Find(cell => cell.Coordinates == coordinates);
        }
        else
        {
            return null;
        }
    }

    public Cell GetCell(int x, int y, int z)
    {
        return GetCell(new Vector3Int(x, y, z));
    }

    public Cell GetRandomCell()
    {
        return GetCell((int)Random.Range(0, _gridSize.x / CellPrefab.transform.localScale.x), 0, (int)Random.Range(0, _gridSize.z / CellPrefab.transform.localScale.z));
    }

    public void DestroyCell(Cell cell)
    {
        if (_cells.Contains(cell)) _cells.Remove(cell);
        Destroy(cell.gameObject);
    }
}
