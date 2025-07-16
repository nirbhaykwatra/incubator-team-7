using System;
using GameEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Camera _fpsCamera;
    [SerializeField] private float InteractionRange = 8f;
    [SerializeField] private LayerMask interactionMask = -1; // What layers can be interacted with

    [Header("UI Feedback")]
    [SerializeField] private GameObject interactionPrompt; // UI element showing "Press E to interact"
    [SerializeField] private Text interactionText; // Text component for item name
    [SerializeField] private string interactionKey = "E";

    [Header("Events")]
    [SerializeField] private GameObjectEventAsset OnInteractableHover;
    [SerializeField] private GameObjectEventAsset OnInteractableClick;

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = true;

    private IInteractable currentInteractable;
    private GameObject currentInteractableObject;
    private PickableObject currentPickable;

    private void Awake()
    {
        if (_fpsCamera == null)
            _fpsCamera = Camera.main;

        // Hide interaction prompt initially
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        RaycastHit hit;
        Ray ray = new Ray(_fpsCamera.transform.position, _fpsCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, InteractionRange, interactionMask))
        {
            // Check if we hit an interactable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // New interactable found
                if (interactable != currentInteractable)
                {

                    currentInteractable = interactable;
                    currentInteractableObject = hit.collider.gameObject;
                    currentPickable = hit.collider.GetComponent<PickableObject>();

                    // Set hover state on new object
                    if (currentPickable != null)
                    {
                        ShowInteractionPrompt(currentPickable.ItemName);
                    }
                    else
                    {
                        ShowInteractionPrompt("Interact");
                    }

                    OnInteractableHover?.Invoke(currentInteractableObject);
                }
            }
            else
            {
                ClearCurrentInteractable();
            }
        }
        else
        {
            ClearCurrentInteractable();
        }
    }

    private void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable = null;
            currentInteractableObject = null;
            currentPickable = null;
            HideInteractionPrompt();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        if (currentInteractable != null)
        {
            // Check if it's already collected (for pickable objects)
            if (currentPickable != null && currentPickable.IsPickedUp)
            {
                Debug.Log("Item already collected!");
                return;
            }

            // Perform the interaction
            currentInteractable.Interact();
            OnInteractableClick?.Invoke(currentInteractableObject);

            // Clear the current interactable if it was picked up
            if (currentPickable != null && currentPickable.IsPickedUp)
            {
                ClearCurrentInteractable();
            }
        }
    }

    private void ShowInteractionPrompt(string itemName)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
            if (interactionText != null)
            {
                interactionText.text = $"Press {interactionKey} to pick up {itemName}";
            }
        }
    }

    private void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugRay) return;

        Gizmos.color = currentInteractable != null ? Color.green : Color.red;
        if (_fpsCamera != null)
        {
            Gizmos.DrawRay(_fpsCamera.transform.position, _fpsCamera.transform.forward * InteractionRange);

            // Draw a sphere at the interaction point
            if (currentInteractableObject != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(currentInteractableObject.transform.position, 0.5f);
            }
        }
    }
}