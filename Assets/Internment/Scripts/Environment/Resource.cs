using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private List<ResourceData> _resourceDataList = new List<ResourceData>();
    private ResourceData _resourceData;
    [ShowInInspector] [ReadOnly]
    private float _resourceHealth;
    [ShowInInspector] [ReadOnly]
    private float _hardness;
    [ShowInInspector] [ReadOnly]
    private float _rarity;
    private MeshFilter _meshFilter;
    
    private Color GetResourceHealthColor(float value)
    {
        return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / _resourceHealth, 2));
    }

    private void OnValidate()
    {
        GetResourceData();
        _hardness = _resourceData.Hardness;
        _rarity = _resourceData.Rarity;
        _resourceHealth = _resourceData.Health;
    }
    
    private void Awake()
    {
        GetResourceData();
        _meshFilter = GetComponent<MeshFilter>();

        if (_resourceDataList.Count > 0 && _resourceData != null)
        {
            _hardness = _resourceData.Hardness;
            _rarity = _resourceData.Rarity;
            _resourceHealth = _resourceData.Health;
            _meshFilter.sharedMesh = _resourceData.Mesh;
            Debug.Log($"Set resource mesh to {_resourceData.Mesh.name}");
            switch (_resourceData.ColliderType)
            {
                case ColliderType.Box:
                    gameObject.AddComponent<BoxCollider>();
                    break;
                case ColliderType.Sphere:
                    gameObject.AddComponent<SphereCollider>();
                    break;
                case ColliderType.Capsule:
                    gameObject.AddComponent<CapsuleCollider>();
                    break;
                case ColliderType.Mesh:
                    gameObject.AddComponent<MeshCollider>();
                    break;
            }
        }
    }

    private void Start()
    {
        
    }

    private void GetResourceData()
    {
        _resourceData = _resourceDataList[UnityEngine.Random.Range(0, _resourceDataList.Count - 1)];
    }

    public bool MineResource()
    {
        _resourceHealth -= _hardness * Time.deltaTime;
        if (_resourceHealth <= 0f)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
