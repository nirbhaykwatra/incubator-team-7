using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBattery : MonoBehaviour
{
    [ProgressBar("_minimumBatteryLevel", "_batteryCapacity", ColorGetter = "GetBatteryColor")]
    private float _batteryLevel = 100f;
    [SerializeField]
    private float _batteryCapacity = 100f;
    private float _minimumBatteryLevel = 0f;
    [SerializeField]
    private float _rechargeRate = 1f;
    [ShowInInspector] [ReadOnly]
    private bool _recharging = false;

    private float totalBatteryConsumption = 0f;
    private List<Equipment> _activeEquipment = new List<Equipment>();

    private Color GetBatteryColor(float value)
    {
        return Color.Lerp(Color.gray, Color.green, Mathf.Pow(value / _batteryCapacity, 2));
    }

    private void Update()
    {
        
    }
    
    public void AddActiveEquipment(Equipment equipment)
    {
        _activeEquipment.Add(equipment);
    }
}
