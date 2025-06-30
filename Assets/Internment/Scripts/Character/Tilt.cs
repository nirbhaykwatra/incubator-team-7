using Unity.Mathematics;
using UnityEngine;

public class Tilt : MonoBehaviour
{
    public float tiltAmount = 5f;
    public float smoothTime = 0.1f;
    private float currentTilt = 0f;
    private Vector3 currentVelocity;


    void Update()
    {


        float horizontalInput = Input.GetAxis("Horizontal");
        float targetTilt = horizontalInput * tiltAmount;

        // Smoothly interpolate towards the target tilt
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref currentVelocity.z, smoothTime);
        transform.localRotation = Quaternion.Euler(0, 0, currentTilt);
    }
}
