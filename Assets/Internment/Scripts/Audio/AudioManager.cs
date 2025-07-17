using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using System.Linq;
using FMOD;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;
    public static AudioManager instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("More than one Audio Manager in scene");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    public EventInstance CreateInstance(EventReference eventReference, Vector3 worldPos)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void PlaySound(EventReference eventReference)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventReference);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.start();
        instance.release();
    }
    private void CleanUp()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
