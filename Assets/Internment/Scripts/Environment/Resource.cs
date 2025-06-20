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

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();

        if (_resourceDataList.Count > 0)
        {
            _hardness = _resourceData.Hardness;
            _rarity = _resourceData.Rarity;
            _resourceHealth = _resourceData.Health;
            _meshFilter.mesh = _resourceData.Mesh;
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

    private void GetResourceData()
    {
        _resourceData = _resourceDataList[UnityEngine.Random.Range(0, _resourceDataList.Count)];
    }

    public void MineResource()
    {
        if (_resourceHealth <= 0f) Destroy(gameObject);
        _resourceHealth -= _hardness * Time.deltaTime;
        Debug.Log($"{_resourceData.Name} health: {_resourceHealth}");
    }
}
