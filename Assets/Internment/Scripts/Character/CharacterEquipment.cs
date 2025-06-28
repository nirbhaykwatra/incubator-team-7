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
        if (_battery && _totalBatteryConsumption > 0f) _battery.Discharge(_totalBatteryConsumption * Time.deltaTime, _totalBatteryConsumption);
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
        switch (equipment._equipmentType)
        {
            case EquipmentType.Active:
                ActiveEquipment.Add(equipment);
                break;
            case EquipmentType.Passive:
                PassiveEquipment.Add(equipment);
                _totalBatteryConsumption += equipment.BatteryConsumptionRate;
                break;
        }
    }

    public void RemoveEquipment(Equipment equipment)
    {
        switch (equipment._equipmentType)
        {
            case EquipmentType.Active:
                ActiveEquipment.Remove(equipment);
                break;
            case EquipmentType.Passive:
                PassiveEquipment.Remove(equipment);
                _totalBatteryConsumption += equipment.BatteryConsumptionRate;
                break;
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
