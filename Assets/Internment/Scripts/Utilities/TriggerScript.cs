using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider entered trigger: " + other.name);
    }
}
