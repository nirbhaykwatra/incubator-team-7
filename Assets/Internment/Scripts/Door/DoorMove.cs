using UnityEngine;

public class DoorMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float distance = 1f;
    public Transform target;

    private bool isTriggered = false;

    private void Update()
    {

        if (isTriggered && target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {

            isTriggered = true;
        }


    }
}
