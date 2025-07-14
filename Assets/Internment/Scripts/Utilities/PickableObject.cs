using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PickableObject : MonoBehaviour, IInteractable
{
    [Header("Pickup Settings")]
    [SerializeField] private string itemName = "Item";
    [SerializeField] private bool destroyOnPickup = true;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private AudioClip pickupSound;

    [ShowInInspector]
    [ReadOnly]
    private bool _isPickedUp;
    private GameObject _player;

    [Header("Events")]
    public UnityEvent<GameObject> OnPickedUp;
    public UnityEvent<string> OnItemCollected;

    private Collider _collider;
    private Rigidbody _rigidbody;
    private MeshRenderer _meshRenderer;

    public string ItemName => itemName;
    public bool IsPickedUp => _isPickedUp;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();

        // Ensure the object has a collider
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider>();
        }

        _collider.isTrigger = false;
    }

    public void Interact()
    {
        if (_isPickedUp)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No player found with 'Player' tag!");
            return;
        }

        PickUp(player);
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
        if (_isPickedUp)
        {
            return;
        }

        _isPickedUp = true;
        Debug.Log($"Player picked up: {itemName}");

        OnPickedUp?.Invoke(player);
        OnItemCollected?.Invoke(itemName);

        // Handle the object after pickup
        if (destroyOnPickup)
        {
            if (_rigidbody != null) _rigidbody.isKinematic = true;
            if (_collider != null) _collider.enabled = false;

            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
