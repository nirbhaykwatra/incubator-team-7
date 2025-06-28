using System;
using UnityEngine;

public class RadarCamera : MonoBehaviour
{ 
    [SerializeField] private Transform _followTarget;
    private Camera _mainCamera;
    private Camera _radarCamera;
    private void Awake()
    {
        _mainCamera = Camera.main;
        _radarCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Vector3 newPosition = _followTarget.position;
        newPosition.y = _radarCamera.transform.position.y;
        _radarCamera.transform.position = newPosition;
        
        _radarCamera.transform.rotation = Quaternion.Euler(90f, _followTarget.eulerAngles.y, 0f);
    }
}
