using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference characterFootsteps { get; private set; }

    [field: Header("Drill SFX")]
    [field: SerializeField] public EventReference drillPowered { get; private set; }

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference elevatorDown { get; private set; }

    [field: Header("Dialogue")]
    [field: SerializeField] public EventReference collectParts { get; private set; }
    [field: SerializeField] public EventReference drillBattery { get; private set; }
    [field: SerializeField] public EventReference elevatorDialogue { get; private set; }
    [field: SerializeField] public EventReference firstPart { get; private set; }
    [field: SerializeField] public EventReference hasToWork { get; private set; }
    [field: SerializeField] public EventReference introDialogue { get; private set; }
    [field: SerializeField] public EventReference mumblingDialogue { get; private set; }
    [field: SerializeField] public EventReference needsToWork { get; private set; }
    [field: SerializeField] public EventReference coughingDialogue { get; private set; }
    [field: SerializeField] public EventReference mumblesV2Dialogue { get; private set; }
    public static FMODEvents instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one FMOD Events instance.");
        }

        instance = this;
    }

    public void TriggerSound()
    {
        AudioManager.instance.PlaySound(introDialogue);
    }

    public void TriggerElevator()
    {
        AudioManager.instance.PlaySound(elevatorDialogue);
    }

    public void PartsAudio()
    {
        AudioManager.instance.PlaySound(collectParts);
    }
}
