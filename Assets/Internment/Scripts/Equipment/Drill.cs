using GameEvents;
using Internment.Digging.Terrain;
using UnityEngine;

public class Drill : Equipment
{
    [SerializeField] protected Camera _fpsCamera;
    [SerializeField] protected float InteractionRange = 5f;
    [SerializeField] protected float digRadius = 2f;
    [SerializeField] protected float drillSpeed;
    [SerializeField] protected FloatEventAsset OnBatterySetup;
    [SerializeField] protected FloatEventAsset OnBatteryUpdate;
    [SerializeField] protected GameObject drillBit;
    public override void Start()
    {
        base.Start();
        _fpsCamera = Camera.main;
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

            drillBit.transform.Rotate(new Vector3(0,0,1) * drillSpeed * Time.deltaTime);
            
            RaycastHit hit;
            
            if (Physics.Raycast(_fpsCamera.gameObject.transform.position, _fpsCamera.gameObject.transform.forward, out hit, InteractionRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out Resource resource))
                {
                    resource.MineResource();
                }

                if (hit.collider.gameObject.TryGetComponent(out Marching marching))
                {
                    int radiusInVoxels = Mathf.CeilToInt(digRadius * marching.resolution);
                    marching.RemoveTerrain(hit.point, radiusInVoxels);
                }
            }
        }

        if (_battery.Recharging && !Powered)
        {
            Recharge();
        }
    }
}
