using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Player/Equipment")]
public class Equipment : ScriptableObject
{
    public bool Powered;
    [Tooltip("Amount of battery consumed per second")]
    public float BatteryConsumptionRate;
    public float BatteryCapacity;
    
    public void PowerOn()
    {
        Powered = true;
    }
    
    public void PowerOff()
    {
        Powered = false;
    }
}
