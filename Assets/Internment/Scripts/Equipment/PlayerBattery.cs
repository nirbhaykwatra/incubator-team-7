using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBattery : MonoBehaviour
{
    [ProgressBar("_minimumBatteryLevel", "_batteryCapacity", ColorGetter = "GetBatteryColor")]
    [SerializeField] private float _batteryLevel = 100f;
    [SerializeField]
    private float _batteryCapacity = 100f;
    private float _minimumBatteryLevel = 0f;
    [SerializeField]
    private float _rechargeRate = 1f;
    [ShowInInspector] [ReadOnly]
    private bool _recharging = true;

    private float _totalBatteryConsumption = 0f;
    [SerializeField] private List<Equipment> _equipment = new List<Equipment>();
    [SerializeField] private Equipment _selectedEquipment;

    private Color GetBatteryColor(float value)
    {
        return Color.Lerp(Color.gray, Color.green, Mathf.Pow(value / _batteryCapacity, 2));
    }

    [Button]
    public void PowerOnTestEquipment()
    {
        PowerOnEquipment(_selectedEquipment);
    }
    
    [Button]
    public void PowerOffTestEquipment()
    {
        PowerOffEquipment(_selectedEquipment);
    }

    private void Awake()
    {
        foreach (Equipment equipment in _equipment)
        {
            equipment.PowerOff();
        }
    }

    private void Update()
    {
        if (_totalBatteryConsumption > 0)
        {
            _batteryLevel -= _totalBatteryConsumption * Time.deltaTime;
        }

        if (_recharging && _batteryLevel < _batteryCapacity)
        {
            _batteryLevel += _rechargeRate * Time.deltaTime;
            _batteryLevel = Mathf.Clamp(_batteryLevel, _minimumBatteryLevel, _batteryCapacity);
        }
        
        
    }
    
    public void AddEquipment(Equipment equipment)
    {
        _equipment.Add(equipment);
    }

    public void RemoveEquipment(Equipment equipment)
    {
        _equipment.Remove(equipment);
    }
    
    public void PowerOnEquipment(Equipment equipment)
    {
        if (_equipment.Contains(equipment))
        {
            equipment.PowerOn();
            _totalBatteryConsumption += equipment.BatteryConsumptionRate;
            Debug.Log($"Powering on {equipment.name}, total consumption: {_totalBatteryConsumption}");
        }
    }

    public void PowerOffEquipment(Equipment equipment)
    {
        if (_equipment.Contains(equipment))
        {
            equipment.PowerOff();
            if (_totalBatteryConsumption > 0) _totalBatteryConsumption -= equipment.BatteryConsumptionRate;
        }
    }
}
