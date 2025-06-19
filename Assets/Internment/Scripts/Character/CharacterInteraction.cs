using System;
using GameEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteraction : MonoBehaviour
{
    [SerializeField] private Camera _fpsCamera;
    [SerializeField] private GameObjectEventAsset OnInteractableHover;
    [SerializeField] private GameObjectEventAsset OnInteractableClick;
    [SerializeField] private float InteractionRange = 5f;
    
    private Vector3 _liftingPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _liftingPosition = Vector3.Cross(transform.forward, Vector3.forward);
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(_fpsCamera.gameObject.transform.position, _fpsCamera.gameObject.transform.forward, out hit, InteractionRange))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                OnInteractableHover.Invoke(gameObject);
            }
        }
    }

    public void OnInteract(InputValue value)
    {
        RaycastHit hit;
        
        if (Physics.Raycast(_fpsCamera.gameObject.transform.position, _fpsCamera.gameObject.transform.forward, out hit, InteractionRange))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                OnInteractableClick.Invoke(gameObject);
                interactable.Interact();
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_fpsCamera.gameObject.transform.position, _fpsCamera.gameObject.transform.forward * InteractionRange);
        Gizmos.DrawSphere(_liftingPosition, 0.1f);
    }
}
