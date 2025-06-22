using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Battery : MonoBehaviour
{
    [ProgressBar("_minimumCapacity", "Capacity", ColorGetter = "GetBatteryColor")]
    public float CurrentLevel = 100f;
    public float Capacity = 100f;
    public float PassiveDrainRate = 1f;
    [field: SerializeField] public bool Recharging { get; set; }
    [field: SerializeField] public bool PassiveDrain { get; set; }
    
    private float _minimumCapacity = 0f;
    
    private Color GetBatteryColor(float value)
    {
        return Color.Lerp(Color.gray, Color.green, Mathf.Pow(value / Capacity, 2));
    }

    private void Update()
    {
        if (PassiveDrain) Discharge(PassiveDrainRate * Time.deltaTime);
    }

    public void Discharge(float dischargeRate, float minimumCapacity = 0f)
    {
        if (CurrentLevel > minimumCapacity) CurrentLevel -= dischargeRate;
    }

    public void Recharge(float rechargeRate)
    {
        if (Recharging && CurrentLevel < Capacity) CurrentLevel += rechargeRate;
    }
}
