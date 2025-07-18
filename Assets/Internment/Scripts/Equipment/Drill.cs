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
    [SerializeField] protected float batteryRewardAmount = 100f;
    [SerializeField] protected bool rewardAsPercentage = false;
    [SerializeField] protected GameObject drillParticles;
    
    private ParticleSystem drillParticlesSystem;
    public override void Start()
    {
        base.Start();
        drillParticles.SetActive(false);
        drillParticlesSystem = drillParticles.GetComponentInChildren<ParticleSystem>();
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
            drillParticles.SetActive(true);
            drillParticlesSystem.Play();
            
            RaycastHit hit;
            
            if (Physics.Raycast(_fpsCamera.gameObject.transform.position, _fpsCamera.gameObject.transform.forward, out hit, InteractionRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out Resource resource))
                {
                    bool resourceDestroyed = resource.MineResource();

                    // If resource was destroyed, add battery power
                    if (resourceDestroyed)
                    {
                        AddBatteryReward();
                    }
                }

                if (hit.collider.gameObject.TryGetComponent(out Marching marching))
                {
                    int radiusInVoxels = Mathf.CeilToInt(digRadius * marching.resolution);
                    marching.RemoveTerrain(hit.point, radiusInVoxels);
                }
            }
        }
        else
        {
            drillParticlesSystem.Stop();
            drillParticles.SetActive(false);
        }

        if (_battery.Recharging && !Powered)
        {
            Recharge();
        }
    }

    private void AddBatteryReward()
    {
        if (_battery == null) return;

        float amountToAdd = batteryRewardAmount;

        if (rewardAsPercentage)
        {
            amountToAdd = _battery.Capacity * (batteryRewardAmount / 100f);
        }

        // Add battery charge
        float previousLevel = _battery.CurrentLevel;
        _battery.CurrentLevel = Mathf.Clamp(
            _battery.CurrentLevel + amountToAdd,
            0f,
            _battery.Capacity
        );

        float actualAmountAdded = _battery.CurrentLevel - previousLevel;

        Debug.Log($"Mining reward: Added {actualAmountAdded:F1} battery charge. Current level: {_battery.CurrentLevel:F1}/{_battery.Capacity:F1}");

        // Update UI
        OnBatteryUpdate?.Invoke(_battery.CurrentLevel);
    }
}
