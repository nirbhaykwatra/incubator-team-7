using UnityEngine;
using UnityEngine.InputSystem;

public enum EquipmentType
{
    Active,
    Passive
}
public abstract class Equipment : MonoBehaviour
{
    public bool Powered;
    [Tooltip("Amount of battery consumed per second")]
    public float BatteryConsumptionRate;
    public Battery _battery;
    public EquipmentType _equipmentType;
    
    public Camera _fpsCamera;
    public float InteractionRange = 5f;

    public virtual void Awake()
    {
        _fpsCamera = Camera.main;
    }
    
    public virtual void PowerOn()
    {
        Powered = true;
    }
    
    public virtual void PowerOff()
    {
        Powered = false;
    }

    public virtual void UseEquipment()
    {
        
    }

    public virtual void UseEquipment(bool pressed)
    {
        
    }

    public virtual void UseBattery(Battery battery)
    {
        _battery = battery;
    }

    public virtual void Recharge()
    {
        _battery.Recharge();
    }

    public virtual void Discharge()
    {
        if (Powered) _battery.Discharge(BatteryConsumptionRate);
    }

    public virtual void EquipmentUpdate()
    {
        
    }
}
