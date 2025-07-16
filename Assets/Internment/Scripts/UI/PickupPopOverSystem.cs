using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Internment.UI
{
    public class PickupPopOverSystem : MonoBehaviour
    {
        [System.Serializable]
        public class PopoverSettings
        {
            public GameObject popoverPrefab; // Prefab with Image and Text components
            public Transform popoverContainer; // Parent transform for popovers
            public float fadeInDuration = 0.3f;
            public float displayDuration = 2f;
            public float fadeOutDuration = 0.5f;
            public float moveUpSpeed = 50f; // Pixels per second
            public Vector2 startOffset = new Vector2(0, -50f);
        }

        [Header("Settings")] [SerializeField] private PopoverSettings settings;

        [Header("Fallback Icon")]
        [SerializeField] private Sprite defaultIcon;

        [Header("Pool Settings")] [SerializeField]
        private int poolSize = 5;

        private Queue<GameObject> popoverPool = new Queue<GameObject>();
        private PlayerInventory playerInventory;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            InitializePool();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<PlayerInventory>();
                if (playerInventory != null)
                {
                    playerInventory.OnItemAddedWithData.AddListener(ShowPickupPopoverWithData);
                }
            }
        }

        private void InitializePool()
        {
            if (settings.popoverPrefab == null || settings.popoverContainer == null)
            {
                Debug.LogError("Popover prefab or container not set!");
                return;
            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject popover = Instantiate(settings.popoverPrefab, settings.popoverContainer);
                popover.SetActive(false);
                popoverPool.Enqueue(popover);
            }
        }

        public void ShowPickupPopoverWithData(PlayerInventory.InventoryItem item)
        {
            if (item == null) return;
            ShowPickupPopover(item.name, item.count, item.icon);
        }

        public void ShowPickupPopover(string itemName, int quantity = 1, Sprite itemIcon = null)
        {
            if (popoverPool.Count == 0)
            {
                Debug.LogWarning("No available popovers in pool!");
                return;
            }

            GameObject popover = popoverPool.Dequeue();
            StartCoroutine(AnimatePopover(popover, itemName, quantity, itemIcon));
        }

        private IEnumerator AnimatePopover(GameObject popover, string itemName, int quantity, Sprite itemIcon)
        {
            // Get components
            RectTransform rectTransform = popover.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = popover.GetComponent<CanvasGroup>();
            Image iconImage = popover.transform.Find("Icon")?.GetComponent<Image>();
            Text itemText = popover.transform.Find("Text")?.GetComponent<Text>();

            // Ensure we have a CanvasGroup
            if (canvasGroup == null)
            {
                canvasGroup = popover.AddComponent<CanvasGroup>();
            }

            // Set content
            if (iconImage != null)
            {
                // Use the provided icon, or fall back to default
                Sprite icon = itemIcon != null ? itemIcon : defaultIcon;
                iconImage.sprite = icon;
                iconImage.enabled = icon != null;
            }

            if (itemText != null)
            {
                itemText.text = $"<color=yellow>{itemName}</color> x{quantity} collected";
            }

            // Setup initial state
            popover.SetActive(true);
            canvasGroup.alpha = 0f;
            rectTransform.anchoredPosition = settings.startOffset;

            // Fade in
            float elapsed = 0f;
            Vector2 startPos = rectTransform.anchoredPosition;

            while (elapsed < settings.fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / settings.fadeInDuration;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            canvasGroup.alpha = 1f;

            // Display and move up
            elapsed = 0f;
            while (elapsed < settings.displayDuration)
            {
                elapsed += Time.deltaTime;
                rectTransform.anchoredPosition += Vector2.up * settings.moveUpSpeed * Time.deltaTime;
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            while (elapsed < settings.fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / settings.fadeOutDuration;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                rectTransform.anchoredPosition += Vector2.up * settings.moveUpSpeed * Time.deltaTime;
                yield return null;
            }

            // Reset and return to pool
            popover.SetActive(false);
            canvasGroup.alpha = 0f;
            popoverPool.Enqueue(popover);
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
            {
                playerInventory.OnItemAddedWithData.RemoveListener(ShowPickupPopoverWithData);
            }
        }
    }
}