using GameEvents;
using UnityEngine;

public class Jetpack : Equipment
{
    [SerializeField] protected float _propulsionForce = 2f;
    [SerializeField] protected FloatEventAsset OnBatterySetup;
    [SerializeField] protected FloatEventAsset OnBatteryUpdate;
    
    private CharacterMovement3D _playerMovement;
    public override void Start()
    {
        base.Start();
        UseBattery(_playerBattery);
        _playerMovement = FindAnyObjectByType<PlayerController>().GetComponent<CharacterMovement3D>();
        OnBatterySetup?.Invoke(_battery.Capacity);
    }
    public override void UseEquipment(bool pressed)
    {
        if (_battery.CurrentLevel > 0f)
        {
            Powered = pressed;
            OnBatteryUpdate?.Invoke(_battery.CurrentLevel);
        }

        if (!pressed)
        {
            _playerMovement.DeactivateJetpack();
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
            _playerMovement.ActivateJetpack(Powered, _propulsionForce);
        }

        if (_battery.Recharging && !Powered)
        {
            Recharge();
        }
    }
}
