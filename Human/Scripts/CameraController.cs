using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 2f;
    public float smoothRotationSpeed = 5f;
    public float rotationAngleX = 0f;

    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;

    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(rotationAngleX, 0f, 0f);
        desiredPosition = target.position + rotation * offset;
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothRotationSpeed * Time.deltaTime);
    }
}
