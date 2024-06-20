using UnityEngine;
using System;

public class ForceGrabObject : MonoBehaviour
{
    public ForceGrabObject otherHandScript;
    public GameObject dummyController;

    [Header("Head Information")]
    public GameObject head;
    public float headAverageWeight = 3;
    public float headRotationAroundY;

    [Header("Grabbed Object Information")]
    public Transform GrabObjectTransformBeforeGrab;
    public GameObject grabObject;
    private Rigidbody grabObjectRigidBody;

    [Header("Hand Information")]
    public Transform handTransformBeforeGrab;
    public bool isGrabbing = false; 
    private Rigidbody handRigidBody;
    public float handOriginalMass;
    
    [Header("Grab Area Scalars")]
    public float forceScalar = 10f;
    public float maxGrabDistance = 100f;
    public float grabDeltaRadius = 0.5f;

    [Header("Offset Parameters (X is Right, Y is Up, Z is Forward)")]
    public Vector3 offsetPower = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 offsetScalarsBeforeExp = new Vector3(2f, 2f, 2f);
    public Vector3 offsetScalarsAfterExp = new Vector3(1f, 1f, 1f);
    public float minimumForwardDistance = 0.5f;

    [Header("Position Vectors")]
    public Vector3 handPositionBeforeGrab;
    public Vector3 handPositionDifference;
    public Vector3 handPositionDifferenceComponents;
    public Vector3 handPositionDifferenceComponentsTransformed;
    public Vector3 handOffsetPosition;
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

                handTransformBeforeGrab = transform;
                handPositionBeforeGrab = handTransformBeforeGrab.position; 
                handRigidBody.mass = grabObjectRigidBody.mass;

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
            //Vector3 headTransformForwardProjected = head.transform.forward - Vector3.Dot(head.transform.forward, Vector3.up) * Vector3.up;
            Vector3 ProjectNormalizedToUpPlane(Vector3 vector) { return vector.normalized - Vector3.Dot(vector.normalized, Vector3.up) * Vector3.up; }
            Vector3 averagePositionsProjected =
                ProjectNormalizedToUpPlane(transform.position - head.transform.position) +
                ProjectNormalizedToUpPlane(head.transform.forward) * headAverageWeight +
                ProjectNormalizedToUpPlane(otherHandScript.gameObject.transform.position - head.transform.position);

            headRotationAroundY = Vector3.SignedAngle(Vector3.forward, averagePositionsProjected, Vector3.up); 
            Vector3 handPositionBeforeGrabTransformed = new Vector3(head.transform.position.x, 0, head.transform.position.z) + Quaternion.AngleAxis(headRotationAroundY, Vector3.up) * handPositionBeforeGrab;
            dummyController.transform.position = handPositionBeforeGrabTransformed;
            handPositionDifference = transform.position - handPositionBeforeGrabTransformed; 

            //Break down the Vector3 into Components
            handPositionDifferenceComponents.x = Vector3.Dot(handPositionDifference, transform.right);
            handPositionDifferenceComponents.y = Vector3.Dot(handPositionDifference, transform.up);
            handPositionDifferenceComponents.z = Vector3.Dot(handPositionDifference, transform.forward);

            //Apply Transformations
            float raiseToPowerAndScaleWithoutLosingSign(float number, float power, float scalar1, float scalar2) { return Mathf.Sign(number) * Mathf.Pow(Mathf.Abs(number * scalar1), power) / scalar2; }
            handPositionDifferenceComponentsTransformed.x = raiseToPowerAndScaleWithoutLosingSign(handPositionDifferenceComponents.x, offsetPower.x, offsetScalarsBeforeExp.x, offsetScalarsAfterExp.x); 
            handPositionDifferenceComponentsTransformed.y = raiseToPowerAndScaleWithoutLosingSign(handPositionDifferenceComponents.y, offsetPower.y, offsetScalarsBeforeExp.y, offsetScalarsAfterExp.y); 
            handPositionDifferenceComponentsTransformed.z = raiseToPowerAndScaleWithoutLosingSign(handPositionDifferenceComponents.z, offsetPower.z, offsetScalarsBeforeExp.z, offsetScalarsAfterExp.z); 

            if (handPositionDifferenceComponentsTransformed.z < minimumForwardDistance) handPositionBeforeGrabTransformed.z = minimumForwardDistance;

            //Reconstruct the Vector3
            handOffsetPosition =
                handPositionDifferenceComponentsTransformed.x * transform.right +
                handPositionDifferenceComponentsTransformed.y * transform.up +
                handPositionDifferenceComponentsTransformed.z * transform.forward;

            rotatedObjectPosition = transform.position + transform.forward * positionDifferenceMagnitude;
            finalObjectPosition = rotatedObjectPosition 
            //handOffsetPosition
            ; 

            Vector3 newVelocity = (finalObjectPosition - grabObject.transform.position) * forceScalar;

            //Debug.Log(newVelocity);
            grabObjectRigidBody.velocity = newVelocity / grabObjectRigidBody.mass;
        }
    }

}
