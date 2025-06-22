using UnityEngine;

public class DoorMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float distance = 1f;
    public Transform target;
    private bool isInteracted = false;



    // Update is called once per frame
    void Update()
    {
        if(target && isInteracted == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }
    
       
        
    }
