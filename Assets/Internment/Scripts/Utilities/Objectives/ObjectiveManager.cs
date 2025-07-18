using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public class ObjectiveItem
{
    public string itemName;
    public GameObject targetObject;
    public string description;
    public bool isCompleted;
}

public class ObjectiveManager : MonoBehaviour
{
    [Header("Objectives")]
    [SerializeField] private List<ObjectiveItem> objectives = new List<ObjectiveItem>();

    [Header("Current Progress")]
    [ShowInInspector, ReadOnly]
    private int currentObjectiveIndex = 0;

    [ShowInInspector, ReadOnly]
    public ObjectiveItem CurrentObjective =>
        currentObjectiveIndex < objectives.Count ? objectives[currentObjectiveIndex] : null;

    [Header("Events")]
    public UnityEvent<ObjectiveItem> OnObjectiveChanged;
    public UnityEvent<ObjectiveItem> OnObjectiveCompleted;
    public UnityEvent OnAllObjectivesCompleted;

    private PlayerInventory playerInventory;

    private static ObjectiveManager instance;
    public static ObjectiveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ObjectiveManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.OnItemAdded.AddListener(OnItemPickedUp);
            }
        }

        if (objectives.Count > 0)
        {
            OnObjectiveChanged?.Invoke(CurrentObjective);
        }
    }

    private void OnItemPickedUp(string itemName)
    {
        // Check if this is the current objective item
        if (CurrentObjective != null && CurrentObjective.itemName == itemName)
        {
            CompleteCurrentObjective();
        }
    }

    private void CompleteCurrentObjective()
    {
        if (CurrentObjective == null)
        {
            return;
        }

        CurrentObjective.isCompleted = true;
        OnObjectiveCompleted?.Invoke(CurrentObjective);

        currentObjectiveIndex++;

        if (currentObjectiveIndex < objectives.Count)
        {
            OnObjectiveChanged?.Invoke(CurrentObjective);
        }
        else
        {
            OnAllObjectivesCompleted?.Invoke();
        }
    }

    public GameObject GetCurrentTargetObject()
    {
        return CurrentObjective?.targetObject;
    }

    public bool IsObjectiveItem(GameObject obj)
    {
        if (obj == null || CurrentObjective == null) return false;
        return obj == CurrentObjective.targetObject;
    }

    public float GetProgress()
    {
        if (objectives.Count == 0) return 1f;
        return (float)currentObjectiveIndex / objectives.Count;
    }

    [Button("Skip Current Objective (Debug)")]
    private void DebugSkipObjective()
    {
        CompleteCurrentObjective();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnItemAdded.RemoveListener(OnItemPickedUp);
        }
    }
}
