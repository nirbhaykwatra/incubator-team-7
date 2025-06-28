using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class ElevatorMoveDown : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 5f;
 
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool hasTriggered = false;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        startPosition = transform.position;
        targetPosition = startPosition - new Vector3(0, distance, 0);
        
    }

    IEnumerator MoveDown()
    {
        float timeElapsed = 0f;
       
        
            while (timeElapsed < 1f)
            {
                float t = timeElapsed;
                Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);
                rb.MovePosition(newPosition);

                timeElapsed += Time.fixedDeltaTime * speed;
                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(targetPosition);
        


    }
    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            StartCoroutine(MoveDown());
            hasTriggered = true;
        }
    }
}
