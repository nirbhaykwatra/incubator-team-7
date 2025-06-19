using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private List<Equipment> _equipment = new List<Equipment>();
    
    private PlayerBattery _playerBattery;

    private void Awake()
    {
        _playerBattery = GetComponent<PlayerBattery>();
    }
    
    public void 
}
