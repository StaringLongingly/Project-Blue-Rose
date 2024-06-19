using UnityEngine;

public class ForceGrabObject : MonoBehaviour
{
    public Transform GrabObjectTransformBeforeGrab, handTransformBeforeGrab;
    public GameObject grabObject;
    public bool isGrabbing = false; 
    public const float forceScalar = 10f;
    public const float maxGrabDistance = 100f;
    public const float grabDeltaRadius = 0.5f;
    public float forwardScalar = 20f;
    public float rightScalar = 20f;
    public float upScalar = 20f;

    public float positionDifferenceMagnitude;


    private Rigidbody rb;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * maxGrabDistance, Color.red);
        bool foundGrabObject = false;
        RaycastHit hitInfo = new RaycastHit();
        for (float i = 1; i <= maxGrabDistance && !foundGrabObject; i++)
        {
            foundGrabObject = foundGrabObject || Physics.SphereCast
            (
                transform.position + transform.forward * (i - 3),
                i * grabDeltaRadius,
                transform.forward * maxGrabDistance,
                out hitInfo,
                LayerMask.GetMask("Grabbable")
            );
        }
        if (foundGrabObject)
        {
            //Debug.Log("Raycast Hit!");
            if (grabObject == null && isGrabbing)
            {
                Debug.Log("Object Force Grabbed!");
                grabObject = hitInfo.collider.gameObject;
                rb = grabObject.GetComponent<Rigidbody>();
                rb.useGravity = false;
                GrabObjectTransformBeforeGrab = grabObject.transform;
                handTransformBeforeGrab = transform;
                positionDifferenceMagnitude = (GrabObjectTransformBeforeGrab.position - transform.position).magnitude;
            }
        }

        //Debug.Log("Raycast Collided");
        if (!isGrabbing && grabObject != null)
        {
            Debug.Log("Object Force Dropped!", grabObject);
            rb.useGravity = true;
            GrabObjectTransformBeforeGrab = grabObject.transform;
            handTransformBeforeGrab = transform;
            grabObject = null;
            rb = null;
        }
        else if (grabObject != null)
        {
            Vector3 handPositionDifference = transform.position - handTransformBeforeGrab.position;

            float forwardComponent = Vector3.Dot(handPositionDifference, transform.forward);
            float upComponent = Vector3.Dot(handPositionDifference, transform.up);
            float rightComponent = Vector3.Dot(handPositionDifference, transform.right);

            forwardComponent *= forwardScalar;
            upComponent *= upScalar;
            rightComponent *= rightScalar;

            Vector3 handoffsetPosition = transform.forward * forwardComponent + transform.right * rightComponent + transform.up * upComponent;
            Vector3 rotatedObjectPosition = transform.position + transform.forward * positionDifferenceMagnitude;
            Vector3 finalObjectPosition = rotatedObjectPosition + handoffsetPosition; 

            Vector3 difference = finalObjectPosition - grabObject.transform.position;
            float magnitude = difference.magnitude;

            Vector3 direction = difference.normalized;
            Vector3 newVelocity = direction * magnitude * forceScalar;
            //Debug.Log(newVelocity);
            rb.velocity = newVelocity / rb.mass;
        }
    }
}
