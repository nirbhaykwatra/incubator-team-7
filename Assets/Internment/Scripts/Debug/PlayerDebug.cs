using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    private PlayerController _playerController;
    private CharacterMovement3D _characterMovement;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _characterMovement = GetComponent<CharacterMovement3D>();
    }

    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }

    public void ToggleRegenerateHealth()
    {
        _characterMovement.RegenHealth = !_characterMovement.RegenHealth;
    }

    public void ToggleRegenerateOxygen()
    {
        _characterMovement.RegenOxygen = !_characterMovement.RegenOxygen;
    }

    public void ToggleRegenerateBattery()
    {
        
    }
}


