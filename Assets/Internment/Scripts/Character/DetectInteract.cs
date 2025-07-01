using UnityEngine;
using UnityEngine.Events;
public class DetectInteract : MonoBehaviour
{
    [SerializeField]

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Code to execute when the 'E' key is pressed
            Debug.Log("E key pressed!");
            // Add your desired logic here, such as interacting with an object
        }
    }
}
