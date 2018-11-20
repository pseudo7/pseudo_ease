using UnityEngine;

public enum SnapType
{
    XZ_POSITIVE_NEGATIVE, XZ_POSITIVE, XZ_NEGATIVE, X_POSITIVE_NEGATIVE, Z_POSITIVE_NEGATIVE, X_POSITIVE, X_NEGATIVE, Z_POSITIVE, Z_NEGATIVE, NONE
}

public class PseudoSnap : MonoBehaviour
{
    public static Vector3 otherObjectPosition, rayCastPosition;
    public static GameObject currentObject;

    public SnapType snapType;
    public bool isStationary;

    static bool isObjectSnapped, isObjectRotated;
    static Quaternion defaultRotation;
    Quaternion sofaToCornerRotation, cornerToSofaRotation;


    Camera mainCam;
    Collider currentCollider;
    int otherObjectYAngle = 180;

    float doubleTapDelay = .5f;
    float lastTimeTap = -1;

    void Start()
    {
        mainCam = Camera.main;
        currentCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sofa") || other.CompareTag("Corner") || other.CompareTag("EndSofa"))
        {
            int cameraRotation = 0;//(int)Camera.main.transform.parent.rotation.eulerAngles.y % 90;
            if (currentObject != other.gameObject)
            {
                //if (other.CompareTag("Corner"))
                otherObjectYAngle = (int)other.transform.rotation.eulerAngles.y;
                //else cornerSofaRotation = 180;
                float xExtent, zExtent;

                bool isXNegative = (xExtent = transform.position.x - other.ClosestPoint(transform.position).x) < 0;
                bool isZNegative = (zExtent = transform.position.z - other.ClosestPoint(transform.position).z) < 0;

                Debug.LogFormat("<color=red>X: {0}</color>", xExtent);
                Debug.LogFormat("<color=red>Z: {0}</color>", zExtent);

                switch (snapType)
                {
                    case SnapType.XZ_POSITIVE_NEGATIVE:
                        bool isLeft, isRight, isForward, isBack;
                        isLeft = isRight = isForward = isBack = false;

                        if (xExtent > Mathf.Abs(zExtent))
                        {
                            otherObjectPosition = other.gameObject.transform.position + Vector3.right * (other.bounds.extents.x + (currentCollider.bounds.extents.x));
                            Debug.LogFormat("Right");
                            isRight = true;
                        }
                        else if (zExtent > Mathf.Abs(xExtent))
                        {
                            otherObjectPosition = other.gameObject.transform.position + Vector3.forward * (other.bounds.extents.z + (currentCollider.bounds.extents.z));
                            Debug.LogFormat("Forward");
                            isForward = true;
                        }
                        else if (xExtent < zExtent)
                        {
                            otherObjectPosition = other.gameObject.transform.position + Vector3.left * (other.bounds.extents.x + (currentCollider.bounds.extents.x));
                            Debug.LogFormat("Left");
                            isLeft = true;
                        }
                        else if (zExtent < xExtent)
                        {
                            otherObjectPosition = other.gameObject.transform.position + Vector3.back * (other.bounds.extents.z + (currentCollider.bounds.extents.z));
                            Debug.LogFormat("Back");
                            isBack = true;
                        }

                        switch (otherObjectYAngle)
                        {
                            case 0:
                                if (isForward)
                                    sofaToCornerRotation = Quaternion.Euler(0, 90 + cameraRotation, 0);
                                if (isRight)
                                    sofaToCornerRotation = Quaternion.Euler(0, 0 + cameraRotation, 0);
                                break;
                            case 90:
                                if (isBack)
                                    sofaToCornerRotation = Quaternion.Euler(0, 90 + cameraRotation, 0);
                                if (isRight)
                                    sofaToCornerRotation = Quaternion.Euler(0, 180 + cameraRotation, 0);
                                break;
                            case 180:
                                if (isBack)
                                    sofaToCornerRotation = Quaternion.Euler(0, 270 + cameraRotation, 0);
                                if (isLeft)
                                    sofaToCornerRotation = Quaternion.Euler(0, 180 + cameraRotation, 0);
                                break;
                            case 270:
                                if (isForward)
                                    sofaToCornerRotation = Quaternion.Euler(0, 270 + cameraRotation, 0);
                                if (isLeft)
                                    sofaToCornerRotation = Quaternion.Euler(0, 0 + cameraRotation, 0);
                                break;
                        }

                        switch (otherObjectYAngle)
                        {
                            case 0:
                                if (isLeft)
                                    cornerToSofaRotation = Quaternion.Euler(0, 0 + cameraRotation, 0);
                                if (isRight)
                                    cornerToSofaRotation = Quaternion.Euler(0, 270 + cameraRotation, 0);
                                break;
                            case 90:
                                if (isForward)
                                    cornerToSofaRotation = Quaternion.Euler(0, 90 + cameraRotation, 0);
                                if (isBack)
                                    cornerToSofaRotation = Quaternion.Euler(0, 0 + cameraRotation, 0);
                                break;
                            case 180:
                                if (isRight)
                                    cornerToSofaRotation = Quaternion.Euler(0, 180 + cameraRotation, 0);
                                if (isLeft)
                                    cornerToSofaRotation = Quaternion.Euler(0, 90 + cameraRotation, 0);
                                break;
                            case 270:
                                if (isBack)
                                    cornerToSofaRotation = Quaternion.Euler(0, 270 + cameraRotation, 0);
                                if (isForward)
                                    cornerToSofaRotation = Quaternion.Euler(0, 180 + cameraRotation, 0);
                                break;
                        }

                        //Debug.LogErrorFormat("<color=white>{0}: {1}</color>", other.name, other.tag);

                        if (!isObjectRotated && currentObject && currentObject.CompareTag("Sofa") && other.CompareTag("Corner"))
                        {
                            isObjectRotated = true;
                            Debug.Log("<color=white>S=>C</color>");
                            currentObject.transform.rotation = sofaToCornerRotation;
                        }
                        else if (!isObjectRotated && currentObject && currentObject.CompareTag("Sofa") && other.CompareTag("Sofa"))
                        {
                            isObjectRotated = true;
                            Debug.Log("<color=white>S=>S</color>");
                            currentObject.transform.rotation = other.transform.rotation;
                        }
                        else if (!isObjectRotated && currentObject && currentObject.CompareTag("Corner") && other.CompareTag("Sofa"))
                        {
                            isObjectRotated = true;
                            Debug.Log("<color=white>C=>S</color>");
                            currentObject.transform.rotation = cornerToSofaRotation;
                        }
                        //isObjectSnapped = true;

                        isLeft = isRight = isForward = isBack = false;
                        break;
                    case SnapType.XZ_POSITIVE:
                        if (Mathf.Abs(xExtent) > Mathf.Abs(zExtent))
                        {
                            otherObjectPosition = other.gameObject.transform.position + Vector3.right * (other.bounds.extents.x + (currentCollider.bounds.extents.x));
                            Debug.LogFormat("Right");
                        }
                        else
                        {
                            otherObjectPosition = other.gameObject.transform.position + Vector3.forward * (other.bounds.extents.z + (currentCollider.bounds.extents.z));
                            Debug.LogFormat("Forward");
                        }
                        break;
                    case SnapType.XZ_NEGATIVE:
                        if (Mathf.Abs(xExtent) > Mathf.Abs(zExtent))
                        {
                            otherObjectPosition = other.gameObject.transform.position + Vector3.left * (other.bounds.extents.x + (currentCollider.bounds.extents.x));
                            Debug.LogFormat("Left");
                        }
                        else
                        {
                            otherObjectPosition = other.gameObject.transform.position + Vector3.back * (other.bounds.extents.z + (currentCollider.bounds.extents.z));
                            Debug.LogFormat("Back");
                        }
                        break;
                    case SnapType.X_POSITIVE_NEGATIVE:
                        otherObjectPosition = other.gameObject.transform.position + (isXNegative ? Vector3.left : Vector3.right) * (other.bounds.extents.x + (currentCollider.bounds.extents.x));
                        Debug.LogFormat((isXNegative ? "Left" : "Right"));
                        break;
                    case SnapType.Z_POSITIVE_NEGATIVE:
                        otherObjectPosition = other.gameObject.transform.position + (isZNegative ? Vector3.back : Vector3.forward) * (other.bounds.extents.z + (currentCollider.bounds.extents.z));
                        Debug.LogFormat((isZNegative ? "Back" : "Forward"));
                        break;
                    case SnapType.X_POSITIVE:
                        otherObjectPosition = other.gameObject.transform.position + Vector3.right * (other.bounds.extents.x + (currentCollider.bounds.extents.x));
                        Debug.LogFormat("Right");
                        break;
                    case SnapType.X_NEGATIVE:
                        otherObjectPosition = other.gameObject.transform.position + Vector3.left * (other.bounds.extents.x + (currentCollider.bounds.extents.x));
                        Debug.LogFormat("Left");
                        break;
                    case SnapType.Z_POSITIVE:
                        otherObjectPosition = other.gameObject.transform.position + Vector3.forward * (other.bounds.extents.z + (currentCollider.bounds.extents.z));
                        Debug.LogFormat("Forward");
                        break;
                    case SnapType.Z_NEGATIVE:
                        otherObjectPosition = other.gameObject.transform.position + Vector3.back * (other.bounds.extents.z + (currentCollider.bounds.extents.z));
                        Debug.LogFormat("Back");
                        break;

                }
                isObjectSnapped = true;
            }
        }
    }


    void Deactivate()
    {
        isObjectSnapped = false;
        isObjectRotated = false;
        currentObject = null;
    }

    private void OnMouseDown()
    {
        currentObject = gameObject;
        if (isStationary)
            return;
        isObjectSnapped = false;
    }

    private void OnMouseUpAsButton()
    {

        if (lastTimeTap < 0)
            lastTimeTap = Time.time;
        else
        if (Time.time - lastTimeTap < doubleTapDelay)
        {
            lastTimeTap = Time.time;

            if (isStationary)
                SignUIManager.toggleSelectSofa(currentObject);
            else
                RotateSofa();
        }
        else
            lastTimeTap = Time.time;
    }

    private void RotateSofa()
    {
        transform.Rotate(Vector3.up, 90);
    }

    private void OnMouseUp()
    {
        if (isStationary)
            return;
        if (currentObject)
        {
            currentObject.transform.position = isObjectSnapped ? otherObjectPosition : rayCastPosition;
            Invoke("Deactivate", Time.deltaTime);
        }
    }

    void OnMouseDrag()
    {
        if (isStationary || Input.touchCount > 1)
            return;
        RaycastHit raycastHit;
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out raycastHit))
            rayCastPosition = transform.position = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z);
    }
}
