using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable
{
    [ShowInInspector]
    [ReadOnly]
    private bool _isPickedUp;
    private GameObject _player;
    public void Interact()
    {
        Debug.Log($"isPickedUp: {_isPickedUp}");
        _isPickedUp = !_isPickedUp;
    }
    
    private void FixedUpdate()
    {
        if (_isPickedUp)
        {
            MoveObjectToPosition(Vector3.Cross(_player.transform.forward, Vector3.forward));
        }
    }

    private void MoveObjectToPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void PickUp(GameObject player)
    {
        _player = _isPickedUp ? null : player;
    }
}
