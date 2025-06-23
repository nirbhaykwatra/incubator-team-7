using System;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentState
{
    Operational,
    Emergency
}

public class CharacterRadar : MonoBehaviour
{
    [SerializeField] private float _longRadarRange = 10f;
    [SerializeField] private float _shortRadarRange = 5f;
    [SerializeField] private float _longRadarCooldown = 1f;
    [SerializeField] private SphereCollider _longRadarCollider;
    [SerializeField] private SphereCollider _shortRadarCollider;
    
    private Collider[] _longRangeColliders = new Collider[100];
    private Collider[] _shortRangeColliders = new Collider[100];

    private void Awake()
    {
        _longRadarCollider.radius = _longRadarRange;
        _shortRadarCollider.radius = _shortRadarRange;
    }

    public void HandleRadar()
    {
        int longRangeResources = Physics.OverlapSphereNonAlloc(transform.position, _longRadarRange, _longRangeColliders, 1 << 7);
        int shortRangeResources = Physics.OverlapSphereNonAlloc(transform.position, _shortRadarRange, _shortRangeColliders, 1 << 7);
        
        Debug.Log($"Long range resources: {longRangeResources}");
        Debug.Log($"Short range resources: {shortRangeResources}");
        
        for (int i = 0; i < longRangeResources; i++)
        {
            
            if (_longRangeColliders[i].TryGetComponent(out Resource resource))
            {
                Debug.Log($"Long Range Collider {i}: {resource.gameObject.name}");
                resource.gameObject.GetComponent<ResourceRadarHandler>().PingResource();
            }
        }
        
        for (int i = 0; i < shortRangeResources; i++)
        {
            if (_shortRangeColliders[i].TryGetComponent(out Resource resource))
            {
                Debug.Log($"Short Range Collider {i}: {resource.gameObject.name}");
                resource.gameObject.GetComponent<ResourceRadarHandler>().PingResource();
            }
        }
    }
}
