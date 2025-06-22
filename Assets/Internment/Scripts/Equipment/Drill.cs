using GameEvents;
using UnityEngine;

public class Drill : Equipment
{
    [SerializeField] protected FloatEventAsset OnBatterySetup;
    [SerializeField] protected FloatEventAsset OnBatteryUpdate;
    public override void Awake()
    {
        base.Awake();
        
        OnBatterySetup?.Invoke(_battery.Capacity);
    }
    public override void UseEquipment(bool pressed)
    {
        if (_battery.CurrentLevel > 0f)
        {
            Powered = pressed;
            OnBatteryUpdate?.Invoke(_battery.CurrentLevel);
        }
    }

    public void ToggleRecharging()
    {
        _battery.Recharging = !_battery.Recharging;
    }
    
    public override void EquipmentUpdate()
    {
        OnBatteryUpdate?.Invoke(_battery.CurrentLevel);
        if (Powered)
        {
            Discharge();
            
            RaycastHit hit;
            
            if (Physics.Raycast(_fpsCamera.gameObject.transform.position, _fpsCamera.gameObject.transform.forward, out hit, InteractionRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out Resource resource))
                {
                    resource.MineResource();
                }
            }
        }

        if (_battery.Recharging && !Powered)
        {
            Recharge();
        }
    }
}
