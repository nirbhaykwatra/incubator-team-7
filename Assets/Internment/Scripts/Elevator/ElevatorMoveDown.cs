using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ElevatorMoveDown : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        startPosition = transform.position;
        targetPosition = startPosition - new Vector3(0, distance, 0);
        StartCoroutine(MoveDown());
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
}
