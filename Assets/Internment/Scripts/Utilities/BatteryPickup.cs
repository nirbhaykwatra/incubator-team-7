using UnityEngine;
using GameEvents;
using Internment.UI;

[RequireComponent(typeof(PickableObject))]
public class BatteryPickup : MonoBehaviour
{
    [Header("Battery Settings")]
    [SerializeField] private float batteryAmount = 25f;
    [SerializeField] private bool isPercentage = true;

    [Header("Events")]
    [SerializeField] private FloatEventAsset OnBatteryPickup;

    private PickableObject pickableObject;

    private void Awake()
    {
        pickableObject = GetComponent<PickableObject>();
        pickableObject.OnPickedUp.AddListener(OnPickedUp);
    }

    private void OnPickedUp(GameObject player)
    {
        // Try to find battery on player
        Battery playerBattery = player.GetComponent<Battery>();
        if (playerBattery == null)
        {
            // Try to find battery in children (equipment, etc.)
            playerBattery = player.GetComponentInChildren<Battery>();
        }

        if (playerBattery != null)
        {
            float amountToAdd = batteryAmount;

            if (isPercentage)
            {
                amountToAdd = playerBattery.Capacity * (batteryAmount / 100f);
            }

            // Add battery charge
            float previousLevel = playerBattery.CurrentLevel;
            playerBattery.CurrentLevel = Mathf.Clamp(
                playerBattery.CurrentLevel + amountToAdd,
                0f,
                playerBattery.Capacity
            );

            float actualAmountAdded = playerBattery.CurrentLevel - previousLevel;

            Debug.Log($"Added {actualAmountAdded:F1} battery charge. Current level: {playerBattery.CurrentLevel:F1}/{playerBattery.Capacity:F1}");

            // Trigger event if assigned
            OnBatteryPickup?.Invoke(playerBattery.CurrentLevel);

            // Optional: Show special feedback for battery pickup
            ShowBatteryPickupFeedback(actualAmountAdded);
        }
        else
        {
            Debug.LogWarning("No Battery component found on player!");
        }
    }

    private void ShowBatteryPickupFeedback(float amountAdded)
    {
        // Find the popover system and show special battery feedback
        PickupPopOverSystem popoverSystem = FindFirstObjectByType<PickupPopOverSystem>();
        if (popoverSystem != null)
        {
            string message = isPercentage ?
                $"Battery +{batteryAmount:F0}%" :
                $"Battery +{amountAdded:F0}";
            popoverSystem.ShowPickupPopover(message, 1);
        }
    }

    private void OnDestroy()
    {
        if (pickableObject != null)
        {
            pickableObject.OnPickedUp.RemoveListener(OnPickedUp);
        }
    }
}
