using UnityEngine;
using Sirenix.OdinInspector;

public class Battery : MonoBehaviour
{
    [ProgressBar("_minimumCapacity", "Capacity", ColorGetter = "GetBatteryColor")]
    public float CurrentLevel = 100f;
    public float Capacity = 100f;
    [field: SerializeField] public float RechargeRate { get; set; }
    [field: SerializeField] public bool Recharging { get; set; }
    
    private float _minimumCapacity = 0f;
    
    private Color GetBatteryColor(float value)
    {
        return Color.Lerp(Color.gray, Color.green, Mathf.Pow(value / Capacity, 2));
    }

    public void Discharge(float dischargeRate, float minimumCapacity = 0f)
    {
        if (CurrentLevel > minimumCapacity) CurrentLevel -= dischargeRate;
    }

    public void Recharge()
    {
        if (Recharging && CurrentLevel < Capacity) CurrentLevel += RechargeRate;
    }
}
