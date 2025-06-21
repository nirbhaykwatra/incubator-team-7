using GameEvents;
using UnityEngine;

public class DebugOptionHandler : MonoBehaviour
{
    [Header("Teleport")] 
    public Transform Platform1Transform;
    public Transform Platform2Transform;
    public Transform Platform3Transform;
    public Transform Platform4Transform;
    public Vector3EventAsset OnTeleport;
    public BoolEventAsset OnToggleHealthRegen;
    public BoolEventAsset OnToggleOxygenRegen;
    public BoolEventAsset OnToggleBatteryRegen;
    
    public void Teleport1()
    {
        OnTeleport.Invoke(Platform1Transform.position);
    }

    public void Teleport2()
    {
        OnTeleport.Invoke(Platform2Transform.position);
    }
    
    public void Teleport3()
    {
        OnTeleport.Invoke(Platform3Transform.position);
    }
    
    public void Teleport4()
    {
        OnTeleport.Invoke(Platform4Transform.position);
    }

    public void ToggleHealthRegen()
    {
        OnToggleHealthRegen.Invoke(true);
    }
    
    public void ToggleOxygenRegen()
    {
        OnToggleOxygenRegen.Invoke(true);
    }
    
    public void ToggleBatteryRegen()
    {
        OnToggleBatteryRegen.Invoke(true);
    }
}
