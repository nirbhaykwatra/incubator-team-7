using UnityEngine;

public class Drill : Equipment
{
    public override void UseEquipment(bool pressed)
    {
        if (_battery.CurrentLevel > 0f ) Powered = pressed;
    }
    
    public override void EquipmentUpdate()
    {
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

        if (_battery.Recharging)
        {
            Recharge();
        }
    }
}
