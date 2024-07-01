using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ObjectExploder : MonoBehaviour
{
    private Rigidbody rb;
    public float velocityThreshold;
    private SimpleParticleSystem simpleParticleSystem;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > velocityThreshold)
        {
           simpleParticleSystem.SpawnAllParticleTypes(); 
        }
    }
}
