using UnityEditor.Callbacks;
using UnityEngine;

public class ForceGrabObject : MonoBehaviour
{
    public Transform GrabObjectTransformBeforeGrab, handTransformBeforeGrab;
    public bool isGrabbing = false;
    public const float forceScalar = 10f;
    public const float maxGrabDistance = 100f;
    public const float xPositionScalar = 2f;
    public const float yPositionScalar = 2f;
    public const float zPositionScalar = 2f;
    private Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, maxGrabDistance))
        {
            GameObject grabObject = hitInfo.collider.gameObject;
            if (!isGrabbing)
            {
                GrabObjectTransformBeforeGrab = grabObject.transform;
                handTransformBeforeGrab = transform;
            }
            else
            {
                Vector3 difference = CalculateNewGrabObjectPositionByHandPosition(CalculateNewGrabObjectPositionByHandRotation(GrabObjectTransformBeforeGrab.position)) - transform.position;
                float magnitude = difference.magnitude;
            
                Vector3 direction = difference.normalized;
                Vector3 newVelocity = direction * magnitude * forceScalar;
                Debug.Log(newVelocity);
                rb.velocity = newVelocity / rb.mass;
            }
        }
    }

    Vector3 CalculateNewGrabObjectPositionByHandRotation(Vector3 oldGrabObjectPosition)
    {
        Vector3 angleDifference = transform.rotation.eulerAngles - handTransformBeforeGrab.eulerAngles;
        Quaternion rotationQuaternion = Quaternion.Euler(angleDifference);

        Vector3 positionDifference = oldGrabObjectPosition - transform.position;
        Vector3 positionDifferenceRotated = rotationQuaternion * positionDifference;
        
        Vector3 newGrabObjectPosition = transform.position + positionDifferenceRotated;
        return newGrabObjectPosition;
    }

    Vector3 CalculateNewGrabObjectPositionByHandPosition(Vector3 oldGrabObjectPosition)
    {
        Vector3 handPositionDifference = transform.position - handTransformBeforeGrab.position;

        handPositionDifference.x *= xPositionScalar;
        handPositionDifference.y *= yPositionScalar;
        handPositionDifference.z *= zPositionScalar;

        Vector3 newGrabObjectPosition = handPositionDifference + oldGrabObjectPosition;
        return newGrabObjectPosition;
    }
}
