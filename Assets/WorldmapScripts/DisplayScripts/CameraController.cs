using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float MaxDistance;
    public float MinDistance;
    public float XSpeed;
    public float YSpeed;
    public int YMinLimit;
    public int YMaxLimit;
    public int ZoomRate;
    public float PanSpeed;
    public float ZoomDampening;
    
    private float _xDeg;
    private float _yDeg;
    private float _currentDistance;
    private float _desiredDistance;
    private Quaternion _currentRotation;
    private Quaternion _desiredRotation;
    private Quaternion _rotation;
    private Vector3 _position;

    void Start()
    {
        _currentDistance = Vector3.Distance(transform.position, target.position);
        _desiredDistance = _currentDistance;

        _position = transform.position;
        _rotation = transform.rotation;
        _currentRotation = transform.rotation;
        _desiredRotation = transform.rotation;

        _xDeg = 210;
        _yDeg = 50;
    }

    void DoOrbit()
    {
        _xDeg += Input.GetAxis("Mouse X") * XSpeed * 0.02f;
        _yDeg -= Input.GetAxis("Mouse Y") * YSpeed * 0.02f;
        
        _yDeg = ClampAngle(_yDeg, YMinLimit, YMaxLimit);
        _desiredRotation = Quaternion.Euler(_yDeg, _xDeg, 0);
        _currentRotation = transform.rotation;

        _rotation = Quaternion.Lerp(_currentRotation, _desiredRotation, Time.deltaTime * ZoomDampening);
        transform.rotation = _rotation;
    }

    void DoPan()
    {
        target.rotation = transform.rotation;
        target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * PanSpeed);
        target.Translate(transform.up * -Input.GetAxis("Mouse Y") * PanSpeed, Space.World);
    }

    void DoZoom()
    {
        _desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * ZoomRate * 0.125f * Mathf.Abs(_desiredDistance);
    }
    
    void Update()
    {
        bool alt = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
        bool shouldPan = Input.GetMouseButton(2);
        bool shouldZoom = Input.GetMouseButton(1) && alt;
        bool shouldOrbit = Input.GetMouseButton(1) && !alt;
        if (shouldOrbit)
        {
            DoOrbit();
        }
        if(shouldPan)
        {
            DoPan();
        }
        if(shouldZoom)
        {
            DoZoom();
        }
        
        _desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * ZoomRate * Mathf.Abs(_desiredDistance);
        _desiredDistance = Mathf.Clamp(_desiredDistance, MinDistance, MaxDistance);
        _currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * ZoomDampening);
        _position = target.position - (_rotation * Vector3.forward * _currentDistance);
        transform.position = _position;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        return Mathf.Clamp(angle, min, max);
    }
}