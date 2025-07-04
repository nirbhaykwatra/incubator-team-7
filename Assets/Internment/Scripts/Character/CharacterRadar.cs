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
    [SerializeField] private float _longRadarCooldown = 5f;
    [SerializeField] private float _shortRadarCooldown = 1f;
    [SerializeField] private float _directionDisplayDistance = 2.5f;
    [SerializeField] private float _resourceMinimumVerticalDistanceForArrowDisplay = 2f;
    [SerializeField] private SphereCollider _longRadarCollider;
    [SerializeField] private SphereCollider _shortRadarCollider;
    [field: SerializeField] public EquipmentState EquipmentState { get; set; } = EquipmentState.Operational;
    
    private Battery _playerBattery;
    private Collider[] _longRangeColliders = new Collider[100];
    private Collider[] _shortRangeColliders = new Collider[100];
    private float _longRadarCooldownTimer;
    private float _shortRadarCooldownTimer;

    public float LongRadarCooldownTimer => _longRadarCooldownTimer;
    public float LongRadarCooldownDuration => _longRadarCooldown;

    private void Awake()
    {
        _longRadarCollider.radius = _longRadarRange;
        _shortRadarCollider.radius = _shortRadarRange;

        _longRadarCooldownTimer = _longRadarCooldown;
        _shortRadarCooldownTimer = _shortRadarCooldown;
        _playerBattery = GetComponent<Battery>();
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
                if (_shortRadarCooldownTimer < _shortRadarCooldown) return;
                int shortRangeResources = Physics.OverlapSphereNonAlloc(transform.position, _shortRadarRange, _shortRangeColliders, 1 << 7);
                for (int i = 0; i < shortRangeResources; i++)
                {
                    if (_shortRangeColliders[i].TryGetComponent(out Resource resource))
                    {
                        if (Vector3.Distance(transform.position, resource.transform.position) <
                            _directionDisplayDistance)
                        {
                            Vector3 direction = resource.transform.position - transform.position;
                            if (direction.magnitude > _resourceMinimumVerticalDistanceForArrowDisplay)
                            {
                                if (IsResourceAbove(resource.transform.position))
                                {
                                    resource.gameObject.GetComponent<ResourceRadarHandler>().PingResourceUp();
                                }
                                else
                                {
                                    resource.gameObject.GetComponent<ResourceRadarHandler>().PingResourceDown();
                                }
                            }
                        }
                        else
                        {
                            resource.gameObject.GetComponent<ResourceRadarHandler>().PingResource();
                        }
                    }
                }
                _shortRadarCooldownTimer = 0f;
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
                }
                
                _shortRadarCooldownTimer += Time.deltaTime;
                if (_shortRadarCooldownTimer >= _shortRadarCooldown)
                {
                    int shortRangeResources = Physics.OverlapSphereNonAlloc(transform.position, _shortRadarRange, _shortRangeColliders, 1 << 7);
                    for (int i = 0; i < shortRangeResources; i++)
                    {
                        if (_shortRangeColliders[i].TryGetComponent(out Resource resource))
                        {
                            if (Vector3.Distance(transform.position, resource.transform.position) <
                                _directionDisplayDistance)
                            {
                                Vector3 direction = resource.transform.position - transform.position;
                                if (direction.magnitude > _resourceMinimumVerticalDistanceForArrowDisplay)
                                {
                                    if (IsResourceAbove(resource.transform.position))
                                    {
                                        resource.gameObject.GetComponent<ResourceRadarHandler>().PingResourceUp();
                                    }
                                    else
                                    {
                                        resource.gameObject.GetComponent<ResourceRadarHandler>().PingResourceDown();
                                    }
                                }
                            }
                            else
                            {
                                resource.gameObject.GetComponent<ResourceRadarHandler>().PingResource();
                            }
                        }
                    }
                    _shortRadarCooldownTimer = 0f;
                }
                break;
            case EquipmentState.Emergency:
                if (_shortRadarCooldownTimer < _shortRadarCooldown)
                {
                    _shortRadarCooldownTimer += Time.deltaTime;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void CheckEquipmentState()
    {
        if (_playerBattery.CurrentLevel < 20f)
        {
            EquipmentState = EquipmentState.Emergency;
        }
        else
        {
            EquipmentState = EquipmentState.Operational;
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
