using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class ForceGrabObject : MonoBehaviour
{
    public ForceGrabObject otherHandScript;
    public GameObject dummyController;
    public GameObject previousHighlightedGrabObject;
    public bool foundGrabObject; 
    public float massScalar = 2f;
    public float angularVelocityWhenNear = 1f;
    public float rotationThreshold = 0.2f;

    [Header("Limiters, X is Minimum, Y is Maximum")] 
    public Vector2 limitYPosition;
    public Vector2 limitMagnitude; 

    [Header("Head Information")]
    public GameObject head;

    [Header("Grabbed Object Information")]
    public Transform GrabObjectTransformBeforeGrab;
    public GameObject grabObject;
    private Rigidbody grabObjectRigidBody;
    private bool grabObjectRigidBodyGravityBeforeGrab; 
    public float finalObjectPositionDistanceFromHand;

    [Header("Hand Information")]
    public Transform handTransformBeforeGrab;
    public bool isLeftHand;
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
    public float maxGrabRadius = 10f;

    [Header("Offset Parameters (X is Right, Y is Up, Z is Forward)")]
    public Vector3 offsetPower = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 offsetScalarsBeforeExp = new Vector3(2f, 2f, 2f);
    public Vector3 offsetScalarsAfterExp = new Vector3(1f, 1f, 1f);

    [Header("Position Vectors")]
    public Vector3 randomRotationDirection;
    public Vector3 headPositionBeforeGrab;
    public Vector3 handPositionBeforeGrab;
    public Vector3 handPositionComponents;

    public Vector3 newHandPosition;

    public Vector3 rotatedObjectPosition;
    public Vector3 finalObjectPosition;
    public float positionDifferenceMagnitude;


    void Start()
    {
        handRigidBody = gameObject.GetComponent<Rigidbody>();
        handOriginalMass = handRigidBody.mass; 
    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * maxGrabDistance, Color.red);
        if (grabObject == null)
        {
            bool foundGrabObject = false;
            RaycastHit hitInfo = new RaycastHit();
            int grabbableLayerMask = LayerMask.GetMask("Grabbable");

            // Inner Loop for back-to-back Spheres, outer Loop for Bigger Radii
            for (float currentRadius = 1; currentRadius <= maxGrabRadius; currentRadius += grabDeltaRadius)
            {
                for (float j = 1; j <= maxGrabDistance; j += grabDeltaRadius)
                {
                    Vector3 origin = transform.position + transform.forward * j;
                    foundGrabObject = Physics.SphereCast(
                        origin,
                        currentRadius,
                        transform.forward,
                        out hitInfo,
                        maxGrabDistance,
                        grabbableLayerMask
                    ) && hitInfo.collider.gameObject != otherHandScript.grabObject && hitInfo.collider.gameObject.layer == 3;

                    if (foundGrabObject)
                    {
                        break;
                    }
                }

                if (foundGrabObject)
                {
                    break;
                }
            }

            if (foundGrabObject)
            {
                Debug.Log("Found Grabbable Object");

                if (!isGrabbing)
                {
                    Debug.Log("Object Highlighted");
                    HighlightGameObject highlightScript = hitInfo.collider.gameObject.GetComponent<HighlightGameObject>();
                    if (isLeftHand) highlightScript.isHighlightedFromLeftHand = true;
                    else highlightScript.isHighlightedFromRightHand = true; 

                    if (hitInfo.collider.gameObject != previousHighlightedGrabObject)
                    {
                        previousHighlightedGrabObject = hitInfo.collider.gameObject;
                    }
                }
                else
                {
                    grabObject = hitInfo.collider.gameObject;
                }
            }
            else if (previousHighlightedGrabObject != null)
            {
                Debug.Log("Object Dehighlighted");
                HighlightGameObject highlightScript = previousHighlightedGrabObject.GetComponent<HighlightGameObject>();
                if (isLeftHand) highlightScript.isHighlightedFromLeftHand = false;
                else highlightScript.isHighlightedFromRightHand = false; 
            }

            if (foundGrabObject && isGrabbing)
            {
                Debug.Log("Object Force Grabbed!");
                grabObjectRigidBody = grabObject.GetComponent<Rigidbody>();
                grabObjectRigidBodyGravityBeforeGrab = grabObjectRigidBody.useGravity;
                grabObjectRigidBody.useGravity = false;

                GrabObjectTransformBeforeGrab = grabObject.transform;

                headPositionBeforeGrab =  head.transform.position;

                handTransformBeforeGrab = transform;
                handPositionBeforeGrab = handTransformBeforeGrab.position; 
                handRigidBody.mass = grabObjectRigidBody.mass / massScalar;

                positionDifferenceMagnitude = (GrabObjectTransformBeforeGrab.position - transform.position).magnitude;

                randomRotationDirection = Random.insideUnitSphere.normalized; 
            }
        }

        
        //Debug.Log("Raycast Collided");
        if (!isGrabbing && grabObject != null && grabObjectRigidBody != null)
        {
            Debug.Log("Object Force Dropped!", grabObject);
            grabObjectRigidBody.useGravity = grabObjectRigidBodyGravityBeforeGrab;
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

            //Calculate Rotations
            rotatedObjectPosition = transform.forward * positionDifferenceMagnitude;
            finalObjectPosition = rotatedObjectPosition + newHandPosition;

            //Limit Y Position
            if (finalObjectPosition.y < limitYPosition.x) finalObjectPosition.y = limitYPosition.x;
            if (finalObjectPosition.y > limitYPosition.y) finalObjectPosition.y = limitYPosition.y;

            //Limit Magnitude
            if (finalObjectPosition.magnitude > limitMagnitude.y)
                finalObjectPosition = finalObjectPosition.normalized * limitMagnitude.y;

            //Rotate The Object if it's too Close
            if (finalObjectPosition.magnitude > limitMagnitude.y + rotationThreshold)
                grabObjectRigidBody.angularVelocity = randomRotationDirection * angularVelocityWhenNear;

            Vector3 newVelocity = (finalObjectPosition - grabObject.transform.position) * forceScalar;

            //Debug.Log(newVelocity);
            grabObjectRigidBody.velocity = newVelocity / grabObjectRigidBody.mass;
        }
    }

}
