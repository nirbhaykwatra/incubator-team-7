using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class CharacterEquipment : MonoBehaviour
{
    public Equipment SelectedEquipment;
    public List<Equipment> ActiveEquipment = new List<Equipment>();
    public List<Equipment> PassiveEquipment = new List<Equipment>();
    
    [SerializeField] private Battery _battery;
    private float _totalBatteryConsumption = 0f;
    
    [SerializeField] private Equipment _testEquipment;

    [Button]
    public void PowerOnTestEquipment()
    {
        PowerOnEquipment(_testEquipment);
    }
    
    [Button]
    public void PowerOffTestEquipment()
    {
        PowerOffEquipment(_testEquipment);
    }
    
    private void Awake()
    {
        foreach (Equipment equipment in ActiveEquipment)
        {
            equipment.PowerOff();
            equipment._fpsCamera = Camera.main;
        }
        
        foreach (Equipment equipment in PassiveEquipment)
        {
            equipment.PowerOff();
            equipment.UseBattery(_battery);
            equipment._fpsCamera = Camera.main;
        }
    }
    
    private void Update()
    {
        if (_battery) _battery.Discharge(_totalBatteryConsumption * Time.deltaTime, _totalBatteryConsumption);
        SelectedEquipment.EquipmentUpdate();
    }
    
    public void UseEquipment(bool pressed)
    {
        if (SelectedEquipment._equipmentType == EquipmentType.Active)
        {
            SelectedEquipment.UseEquipment(pressed);
        }
    }
        
    public void AddEquipment(Equipment equipment)
    {
        ActiveEquipment.Add(equipment);
    }

    public void RemoveEquipment(Equipment equipment)
    {
        ActiveEquipment.Remove(equipment);
    }
    public void PowerOnEquipment(Equipment equipment)
    {
        if (ActiveEquipment.Contains(equipment))
        {
            equipment.PowerOn();
            _totalBatteryConsumption += equipment.BatteryConsumptionRate;
            Debug.Log($"Powering on {equipment.name}, total consumption: {_totalBatteryConsumption}");
        }
    }

    public void PowerOffEquipment(Equipment equipment)
    {
        if (ActiveEquipment.Contains(equipment))
        {
            equipment.PowerOff();
            if (_totalBatteryConsumption > 0) _totalBatteryConsumption -= equipment.BatteryConsumptionRate;
        }
    }
    
    public void SelectEquipment(Equipment equipment)
    {
        if (ActiveEquipment.Contains(equipment)) SelectedEquipment = equipment;
    }
    
    public void SelectEquipment(int index)
    {
        SelectedEquipment = ActiveEquipment[index];
    }
    
    public void DeselectEquipment()
    {
        SelectedEquipment = null;
    }
}
