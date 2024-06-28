using System.Linq;
using UnityEngine;

public class HandCalibrator : MonoBehaviour
{
    public GameObject controllerGrabRotationOffset;
    public GameObject controllerOffset;
    public GameObject vectorPrefab;
    public Vector3[] controllerOffsetTransformNormalsBeforePinch;
    public Vector3[] controllerOffsetTransformNormalsDifference; 
    private Vector3[] controllerOffsetTransformNormalsTransformed; 
    private Vector3[] controllerOffsetTransformNormalsBeforePinchTransformed; 
    public GameObject[] vectorGameObjects;
    public Vector3 controllerOffsetPositionBeforePinch;
    public Quaternion controllerOffsetRotationBeforePinch;
    private UpdateHandModelTransform updateHandModelTransform; 
    public float interpolationT;
    public float vectorCorrectionScalar;
    public float vectorDistance = 0.3f;

    void Start()
    {
        controllerOffsetTransformNormalsBeforePinch = new Vector3[4];
        vectorGameObjects = new GameObject[4];
        controllerOffsetTransformNormalsDifference = new Vector3[4]; 
        controllerOffsetTransformNormalsTransformed = new Vector3[4];
        controllerOffsetTransformNormalsBeforePinchTransformed = new Vector3[4];
    }

    public void CalibrateGrabbingDirection()
    {
        Debug.Log("Calibrating Hand ..");
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
        Quaternion targetRotationInterpolated = Quaternion.Slerp(Quaternion.identity, targetRotation, interpolationT);

        controllerGrabRotationOffset.transform.rotation = targetRotationInterpolated;
    }

    public void CalibratePositionAndRotationOffset()
    {
        //Visualization With 3 Vectors

        destroyVectorObjects();

        Vector3[] controllerOffsetTransformNormals = new Vector3[4];
        controllerOffsetTransformNormals[0] = controllerOffset.transform.right;
        controllerOffsetTransformNormals[1] = controllerOffset.transform.up;
        controllerOffsetTransformNormals[2] = controllerOffset.transform.forward;
        controllerOffsetTransformNormals[3] = new Vector3(0, 0, 0);

        foreach (var normalIndex in Enumerable.Range(0, controllerOffsetTransformNormals.Length)) 
        {

            controllerOffsetTransformNormalsTransformed[normalIndex] = controllerOffsetTransformNormals[normalIndex] * vectorDistance + controllerOffset.transform.position;
            controllerOffsetTransformNormalsBeforePinchTransformed[normalIndex] = controllerOffsetTransformNormalsBeforePinch[normalIndex] * vectorDistance + controllerOffsetPositionBeforePinch;
            
            controllerOffsetTransformNormalsDifference[normalIndex] = controllerOffsetTransformNormalsBeforePinchTransformed[normalIndex] - controllerOffsetTransformNormalsTransformed[normalIndex]; 

            vectorGameObjects[normalIndex] = Instantiate(vectorPrefab);
            vectorGameObjects[normalIndex].transform.position = controllerOffsetTransformNormalsTransformed[normalIndex];
            vectorGameObjects[normalIndex].transform.up = -controllerOffsetTransformNormalsDifference[normalIndex].normalized;

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            Color vectorColor = new Color(1, 1, 1);
            switch (normalIndex)
            {
                case 0:
                    vectorColor = new Color(1, 0, 0);
                break;
                case 1:
                    vectorColor = new Color(0, 1, 0);
                break;
                case 2:
                    vectorColor = new Color(0, 0, 1);
                break;    
                case 3:
                    vectorColor = new Color(1, 1, 1);
                break;
            }
            propertyBlock.SetColor("_Color", vectorColor);
            vectorGameObjects[normalIndex].GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
            vectorGameObjects[normalIndex].GetComponent<LineRenderer>().SetPropertyBlock(propertyBlock);
            vectorGameObjects[normalIndex].GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, -controllerOffsetTransformNormalsDifference[normalIndex].magnitude * vectorCorrectionScalar, 0));
        }
    }

    public void getControllerTransformBeforePinch()
    {
        Debug.Log("Got Controller Offset Transform Normals Before Pinch ..");
        controllerOffsetTransformNormalsBeforePinch[0] = controllerOffset.transform.right;
        controllerOffsetTransformNormalsBeforePinch[1] = controllerOffset.transform.up;
        controllerOffsetTransformNormalsBeforePinch[2] = controllerOffset.transform.forward;
        controllerOffsetTransformNormalsBeforePinch[3] = new Vector3(0, 0, 0);

        controllerOffsetPositionBeforePinch = controllerOffset.transform.position;

        controllerOffsetRotationBeforePinch = controllerOffset.transform.rotation;
    }
    
    public void updateHandOffset()
    {
        Debug.Log("Updating Hand Offset");
        destroyVectorObjects();
        controllerOffset.transform.localPosition = controllerOffsetPositionBeforePinch - controllerOffset.transform.position;
        //controllerOffset.transform.localRotation = Quaternion.Inverse(controllerOffset.transform.rotation * Quaternion.Inverse(controllerOffsetRotationBeforePinch));

        PlayerPrefsControllerManager playerPrefsControllerManager = controllerOffset.GetComponent<PlayerPrefsControllerManager>();
        if (playerPrefsControllerManager.isLeftHand)
        {
           playerPrefsControllerManager.SaveQuaternion("Left Hand Rotation Offset", controllerOffset.transform.localRotation);
           playerPrefsControllerManager.SaveVector3("Left Hand Position Offset", controllerOffset.transform.position);
        }
        else
        {
           playerPrefsControllerManager.SaveQuaternion("Right Hand Rotation Offset", controllerOffset.transform.localRotation);
           playerPrefsControllerManager.SaveVector3("Right Hand Position Offset", controllerOffset.transform.position);
        }
    }

    void destroyVectorObjects()
    {
        foreach (var vectorGameObject in vectorGameObjects)
            if (vectorGameObject != null)
                DestroyImmediate(vectorGameObject);
    }
}
