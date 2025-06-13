using UnityEngine;
using Internment.Digging.Terrain;

namespace Internment.Digging.TestCamera
{
    public class FlyingCamera : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float lookSensitivity = 1f;
        [SerializeField] private float digRadius = 2f;

        private void Update()
        {
            HandleMovement();
            HandleLook();
            HandleDigging();
        }

        private void HandleMovement()
        {
            Vector3 moveDirection = (cam.transform.forward * Input.GetAxis("Vertical") +
                                     transform.right * Input.GetAxis("Horizontal")) * moveSpeed * Time.deltaTime;
            transform.position += moveDirection;
        }

        private void HandleLook()
        {
            float yaw = Input.GetAxis("Mouse X") * lookSensitivity;
            float pitch = -Input.GetAxis("Mouse Y") * lookSensitivity;

            transform.Rotate(Vector3.up, yaw);
            cam.transform.Rotate(Vector3.right, pitch);
        }

        private void HandleDigging()
        {
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
            {
                return;
            }

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            if (!Physics.Raycast(ray, out var hit))
            {
                return;
            }

            if (!hit.transform.CompareTag("Terrain"))
            {
                return;
            }

            Marching marching = hit.transform.GetComponent<Marching>();
            if (marching == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                marching.PlaceTerrain(hit.point);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                marching.RemoveTerrain(hit.point, (int)digRadius);
            }
        }
    }
}