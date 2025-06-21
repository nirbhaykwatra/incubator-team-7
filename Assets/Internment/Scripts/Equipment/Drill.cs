using UnityEngine;

public class Drill : Equipment
{
    private bool _mining = false;
    public override void UseEquipment(bool pressed)
    {
        _mining = pressed;
        Powered = pressed;
    }
    
    public override void EquipmentUpdate()
    {
        if (_mining)
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
    }
}
