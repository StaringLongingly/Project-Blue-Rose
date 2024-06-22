using UnityEngine;

public class ForceGrabObject : MonoBehaviour
{
    public ForceGrabObject otherHandScript;
    public GameObject dummyController;
    public float massScalar = 2f;

    [Header("Limiters, X is Minimum, Y is Maximum")] 
    public Vector2 limitYPosition;
    public Vector2 limitMagnitude; 

    [Header("Head Information")]
    public GameObject head;

    [Header("Grabbed Object Information")]
    public Transform GrabObjectTransformBeforeGrab;
    public GameObject grabObject;
    private Rigidbody grabObjectRigidBody;
    public float finalObjectPositionDistanceFromHand;

    [Header("Hand Information")]
    public Transform handTransformBeforeGrab;
    public bool isGrabbing = false; 
    private Rigidbody handRigidBody;
    public float handOriginalMass;
    public float handDistanceFromHeadBeforeGrab;
    public float handDistanceFromHead;
    public float handDistanceFromHeadDifference;
    public float handDistanceFromHeadDifferenceTransformed;
    
    [Header("Grab Area Scalars")]
    public float forceScalar = 10f;
    public float maxGrabDistance = 100f;
    public float grabDeltaRadius = 0.5f;

    [Header("Offset Parameters (X is Right, Y is Up, Z is Forward)")]
    public Vector3 offsetPower = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 offsetScalarsBeforeExp = new Vector3(2f, 2f, 2f);
    public Vector3 offsetScalarsAfterExp = new Vector3(1f, 1f, 1f);

    [Header("Position Vectors")]
    public Vector3 headPositionBeforeGrab;
    public Vector3 handPositionBeforeGrab;
    public Vector3 handPositionComponents;

    public Vector3 newHandPosition;

    public Vector3 rotatedObjectPosition;
    public Vector3 finalObjectPosition;
    public float grabObjectDistanceToHead;
    public Vector3 finalObjectPositionDeconstructed;
    public Vector3 finalObjectPositionDeconstructedCorrection;
    public float finalObjectPositionProjectedToHead; 
    public float positionDifferenceMagnitude;


    void Start()
    {
        handRigidBody = gameObject.GetComponent<Rigidbody>();
        handOriginalMass = handRigidBody.mass; 
    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * maxGrabDistance, Color.red);
            //Debug.Log("Raycast Hit!");
        if (grabObject == null && isGrabbing)
        {
            bool foundGrabObject = false;
            RaycastHit hitInfo = new RaycastHit();
            for (float i = 1; i <= maxGrabDistance && (!foundGrabObject || foundGrabObject && grabObject == otherHandScript.grabObject); i++)
            {
                foundGrabObject = Physics.SphereCast
                (
                    transform.position + transform.forward * (i - 3),
                    i * grabDeltaRadius,
                    transform.forward * maxGrabDistance,
                    out hitInfo,
                    LayerMask.GetMask("Grabbable")
                );

                if (foundGrabObject) grabObject = hitInfo.collider.gameObject;
            }

            if (foundGrabObject)
            {
                Debug.Log("Object Force Grabbed!");
                grabObjectRigidBody = grabObject.GetComponent<Rigidbody>();
                grabObjectRigidBody.useGravity = false;

                GrabObjectTransformBeforeGrab = grabObject.transform;

                headPositionBeforeGrab =  head.transform.position;

                handTransformBeforeGrab = transform;
                handPositionBeforeGrab = handTransformBeforeGrab.position; 
                handRigidBody.mass = grabObjectRigidBody.mass / massScalar;

                positionDifferenceMagnitude = (GrabObjectTransformBeforeGrab.position - transform.position).magnitude;
            }
        }

        //Hacky Solution, will use this until I find a proper way to prevent non Grabbable objects to be grabbed
        if (grabObject != null && grabObject.layer != 3) grabObject = null;
        
        //Debug.Log("Raycast Collided");
        if (!isGrabbing && grabObject != null && grabObjectRigidBody != null)
        {
            Debug.Log("Object Force Dropped!", grabObject);
            grabObjectRigidBody.useGravity = true;
            grabObject = null;
            grabObjectRigidBody = null;

            handRigidBody.mass = handOriginalMass;
        }
        else if (grabObject != null && grabObjectRigidBody != null)
        {
            //Calculate Magnitude Difference
            Vector3 ProjectToUpPlane(Vector3 vector) { return vector - Vector3.Dot(vector, Vector3.up) * Vector3.up; }
            handDistanceFromHeadBeforeGrab = (ProjectToUpPlane(handPositionBeforeGrab) - ProjectToUpPlane(headPositionBeforeGrab)).magnitude;
            handDistanceFromHead = (ProjectToUpPlane(transform.position) - ProjectToUpPlane(head.transform.position)).magnitude;
            handDistanceFromHeadDifference = handDistanceFromHead - handDistanceFromHeadBeforeGrab;

            //Break down the Vector3 into Components
            handPositionComponents.x = Vector3.Dot(transform.position, transform.right);
            handPositionComponents.y = Vector3.Dot(transform.position, transform.up);
            handPositionComponents.z = Vector3.Dot(transform.position, transform.forward);

            //Apply Transformations
            handDistanceFromHeadDifferenceTransformed = Mathf.Sign(handDistanceFromHeadDifference) * Mathf.Pow(Mathf.Abs(handDistanceFromHeadDifference * offsetScalarsBeforeExp.z), offsetPower.z) * offsetScalarsAfterExp.z;
            //Limit the Distance
            finalObjectPositionDistanceFromHand = handDistanceFromHeadDifferenceTransformed + positionDifferenceMagnitude;
            if (finalObjectPositionDistanceFromHand < limitMagnitude.x) handDistanceFromHeadDifferenceTransformed = limitMagnitude.x - positionDifferenceMagnitude; 

            //Reconstruct the Vector3
            newHandPosition =
                handPositionComponents.x * transform.right +
                handPositionComponents.y * transform.up +
                (handPositionComponents.z + handDistanceFromHeadDifferenceTransformed) * transform.forward;

            rotatedObjectPosition = transform.forward * positionDifferenceMagnitude;
            finalObjectPosition = rotatedObjectPosition + newHandPosition;

            //Limit Y Position
            if (finalObjectPosition.y < limitYPosition.x) finalObjectPosition.y = limitYPosition.x;
            if (finalObjectPosition.y > limitYPosition.y) finalObjectPosition.y = limitYPosition.y;

            //Limit Magnitude
            if (finalObjectPosition.magnitude > limitMagnitude.y) finalObjectPosition = finalObjectPosition.normalized * limitMagnitude.y;

            Vector3 newVelocity = (finalObjectPosition - grabObject.transform.position) * forceScalar;

            //Debug.Log(newVelocity);
            grabObjectRigidBody.velocity = newVelocity / grabObjectRigidBody.mass;
        }
    }

}
