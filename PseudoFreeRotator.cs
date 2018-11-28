using UnityEngine;

public class PseudoFreeRotator : MonoBehaviour
{
    float rotationSpeed = 2.5f;

    void OnMouseDrag()
    {
        float XaxisRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.Rotate(Vector3.down, XaxisRotation, Space.World);
        transform.Rotate(Vector3.right, YaxisRotation, Space.World);
    }
}
