using Unity.Mathematics;
using UnityEngine;

public class Tilt : MonoBehaviour
{
    public float tiltAngle = 30f;
    public float smoothtilt = 2f;
    private float tiltX = 0f;

    

    void Update()
    {
        tiltX = Input.GetAxis("Horizontal");
        
        Quaternion target = Quaternion.Euler(0,0, tiltX * tiltAngle);

        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smoothtilt);
    }
}
