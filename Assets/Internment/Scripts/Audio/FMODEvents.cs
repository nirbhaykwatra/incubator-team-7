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
  public static FMODEvents instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one FMOD Events instance.");
        }

        instance = this;
    }
}
