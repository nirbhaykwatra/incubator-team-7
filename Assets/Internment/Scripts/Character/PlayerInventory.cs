using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItem
    {
        public string name;
        public Sprite icon;
        public int count;

        public InventoryItem(string name, Sprite icon)
        {
            this.name = name;
            this.icon = icon;
            this.count = 1;
        }
    }

    [Header("Inventory Settings")]
    [SerializeField] private int maxItems = 20;

    [ShowInInspector]
    [ReadOnly]
    private List<string> collectedItems = new List<string>();

    [ShowInInspector]
    [ReadOnly]
    private Dictionary<string, InventoryItem> itemDatabase = new Dictionary<string, InventoryItem>();

    [Header("Events")]
    public UnityEvent<string> OnItemAdded;
    public UnityEvent<InventoryItem> OnItemAddedWithData;
    public UnityEvent<int> OnInventoryCountChanged;
    public UnityEvent OnInventoryFull;

    public int CurrentItemCount => collectedItems.Count;
    public bool IsFull => collectedItems.Count >= maxItems;

    
    private void Start()
    {
        OnInventoryCountChanged?.Invoke(0);
    }

    public bool AddItem(PickableObject item)
    {
        if (item == null)
        {
            return false;
        }

        if (IsFull)
        {
            Debug.LogWarning("Inventory is full!");
            OnInventoryFull?.Invoke();
            return false;
        }

        string itemName = item.ItemName;
        Sprite itemIcon = item.ItemIcon;
        collectedItems.Add(itemName);

        if (itemDatabase.ContainsKey(itemName))
        {
            itemDatabase[itemName].count++;
        }
        else
        {
            itemDatabase[itemName] = new InventoryItem(itemName, itemIcon);
        }

        Debug.Log($"Added {itemName} to inventory. Total: {itemDatabase[itemName].count}");

        OnItemAdded?.Invoke(itemName);
        OnItemAddedWithData?.Invoke(itemDatabase[itemName]);
        OnInventoryCountChanged?.Invoke(CurrentItemCount);

        return true;
    }

    public bool HasItem(string itemName)
    {
        return itemDatabase.ContainsKey(itemName) && itemDatabase[itemName].count > 0;
    }

    public int GetItemCount(string itemName)
    {
        return itemDatabase.ContainsKey(itemName) ? itemDatabase[itemName].count : 0;
    }

    public InventoryItem GetItemData(string itemName)
    {
        return itemDatabase.ContainsKey(itemName) ? itemDatabase[itemName] : null;
    }

    public List<string> GetAllItems()
    {
        return new List<string>(collectedItems);
    }

    public Dictionary<string, InventoryItem> GetItemDatabase()
    {
        return new Dictionary<string, InventoryItem>(itemDatabase);
    }

    [Button("Debug: Print Inventory")]
    private void DebugPrintInventory()
    {
        Debug.Log($"Inventory ({CurrentItemCount}/{maxItems})");
        foreach (var kvp in itemDatabase)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
    }
}
