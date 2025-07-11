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
    public Battery _playerBattery;
    public float BatteryRechargeRate;
    public EquipmentType _equipmentType;
    public bool UsePlayerBattery;

    public virtual void Start()
    {
        _battery = GetComponent<Battery>();
        
        // Why is this null in build?
        //_playerBattery = FindAnyObjectByType<PlayerController>().gameObject.GetComponent<Battery>();
        if (UsePlayerBattery && _playerBattery) UseBattery(FindAnyObjectByType<PlayerController>().gameObject.GetComponent<Battery>());
        Debug.Log($"Player Battery: {FindAnyObjectByType<PlayerController>().gameObject.GetComponent<Battery>().Capacity}");
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
        if (_battery == null) return;
        _battery.Recharge(BatteryRechargeRate * Time.deltaTime);
    }

    public virtual void Discharge()
    {
        if (_battery == null) return;
        if (_battery.CurrentLevel <= 0) Powered = false;
        if (Powered) _battery.Discharge(BatteryConsumptionRate * Time.deltaTime);
    }

    public virtual void EquipmentUpdate()
    {
        
    }
}
