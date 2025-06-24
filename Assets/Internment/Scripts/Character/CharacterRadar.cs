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
    [SerializeField] private float _shortRadarCooldown = 1f;
    [SerializeField] private float _directionDisplayDistance = 1f;
    [SerializeField] private SphereCollider _longRadarCollider;
    [SerializeField] private SphereCollider _shortRadarCollider;
    [field: SerializeField] public EquipmentState EquipmentState { get; set; } = EquipmentState.Operational;
    
    private Collider[] _longRangeColliders = new Collider[100];
    private Collider[] _shortRangeColliders = new Collider[100];
    private float _longRadarCooldownTimer;
    private float _shortRadarCooldownTimer;

    private void Awake()
    {
        _longRadarCollider.radius = _longRadarRange;
        _shortRadarCollider.radius = _shortRadarRange;

        _longRadarCooldownTimer = _longRadarCooldown;
    }

    public void HandleRadar()
    {
        switch (EquipmentState)
        {
            case EquipmentState.Operational:
                if (_longRadarCooldownTimer < _longRadarCooldown) return;
                int longRangeResources = Physics.OverlapSphereNonAlloc(transform.position, _longRadarRange, _longRangeColliders, 1 << 7);
                for (int i = 0; i < longRangeResources; i++)
                {
                    if (_longRangeColliders[i].TryGetComponent(out Resource resource))
                    {
                        resource.gameObject.GetComponent<ResourceRadarHandler>().PingResource();
                    }
                }
                _longRadarCooldownTimer = 0f;
                break;
            case EquipmentState.Emergency:
                int shortRangeResources = Physics.OverlapSphereNonAlloc(transform.position, _shortRadarRange, _shortRangeColliders, 1 << 7);
                for (int i = 0; i < shortRangeResources; i++)
                {
                    if (_shortRangeColliders[i].TryGetComponent(out Resource resource))
                    {
                        if (Vector3.Distance(transform.position, resource.transform.position) <
                            _directionDisplayDistance)
                        {
                            if (IsResourceAbove(resource.transform.position))
                            {
                                Debug.Log("Resource is above");
                                resource.gameObject.GetComponent<ResourceRadarHandler>().PingResourceUp();
                            }
                            else
                            {
                                Debug.Log("Resource is below");
                                resource.gameObject.GetComponent<ResourceRadarHandler>().PingResourceDown();
                            }
                        }
                        else
                        {
                            Debug.Log($"Short Range Collider {i}: {resource.gameObject.name}");
                            resource.gameObject.GetComponent<ResourceRadarHandler>().PingResource();
                        }
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        switch (EquipmentState)
        {
            case EquipmentState.Operational:
                if (_longRadarCooldownTimer < _longRadarCooldown)
                {
                    _longRadarCooldownTimer += Time.deltaTime;
                    Debug.Log($"Long Radar Cooldown: {_longRadarCooldownTimer}");
                }
                
                _shortRadarCooldownTimer += Time.deltaTime;
                if (_shortRadarCooldownTimer >= _shortRadarCooldown)
                {
                    int shortRangeResources = Physics.OverlapSphereNonAlloc(transform.position, _shortRadarRange, _shortRangeColliders, 1 << 7);
                    for (int i = 0; i < shortRangeResources; i++)
                    {
                        if (_shortRangeColliders[i].TryGetComponent(out Resource resource))
                        {
                            Debug.Log($"{resource.name} distance from player: {Vector3.Distance(transform.position, resource.transform.position)}");
                            if (Vector3.Distance(transform.position, resource.transform.position) <
                                _directionDisplayDistance)
                            {
                                Debug.Log("Resource is close");
                                Vector3 direction = resource.transform.position - transform.position;
                                if (direction.magnitude > 2f)
                                {
                                    if (IsResourceAbove(resource.transform.position))
                                    {
                                        Debug.Log("Resource is above");
                                        resource.gameObject.GetComponent<ResourceRadarHandler>().PingResourceUp();
                                    }
                                    else
                                    {
                                        Debug.Log("Resource is below");
                                        resource.gameObject.GetComponent<ResourceRadarHandler>().PingResourceDown();
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log($"Pinging Resource: {resource.name}");
                                resource.gameObject.GetComponent<ResourceRadarHandler>().PingResource();
                            }
                        }
                    }
                    _shortRadarCooldownTimer = 0f;
                }
                break;
            case EquipmentState.Emergency:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool IsResourceAbove(Vector3 resourcePosition)
    {
        Vector3 direction = resourcePosition - transform.position;
        Vector3 playerUp = transform.up;
        
        float dotProduct = Vector3.Dot(direction, playerUp);
        
        return dotProduct > 0;
    }
}
