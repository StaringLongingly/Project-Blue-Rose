using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UpdateHandModelTransform : MonoBehaviour
{
    public Transform targetController;
    public float snapPositionThreshold = 0.01f;
    public float velocityThreshold = 0.1f;
    public float ForceScalar = 20f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing.");
        }
    }

    void Update()
    {
        if (rb != null)
        {
            UpdateHandModelPosition();
            rb.MoveRotation(targetController.rotation);
        }
    }

    void UpdateHandModelPosition()
    {
        Vector3 difference = targetController.position - gameObject.transform.position;
        float magnitude = difference.magnitude;
        
        if (magnitude < snapPositionThreshold && 
            targetController.gameObject.GetComponent<VelocityEstimator>().GetVelocityEstimate().magnitude < velocityThreshold)
        {
            rb.MovePosition(targetController.position);
            rb.velocity = new Vector3(0, 0, 0);
        }
        else
        {
            Vector3 direction = difference.normalized;
            Vector3 newVelocity = direction * magnitude * ForceScalar;
            Debug.Log(newVelocity);
            rb.velocity = newVelocity / rb.mass; 
        }
    }
}