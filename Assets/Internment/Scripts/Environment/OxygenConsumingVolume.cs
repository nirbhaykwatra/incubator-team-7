using System;
using UnityEngine;

public class OxygenConsumingVolume : MonoBehaviour
{
    [SerializeField] private float _oxygenConsumptionRate = 1f;
    
    private float _originalOxygenConsumptionRate = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovement3D player))
        {
            _originalOxygenConsumptionRate = player.GetOxygenConsumptionRate();
            player.SetOxygenConsumptionRate(_oxygenConsumptionRate);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovement3D player))
        {
            player.SetOxygenConsumptionRate(_originalOxygenConsumptionRate);
            _originalOxygenConsumptionRate = 0f;
        }
    }
}
