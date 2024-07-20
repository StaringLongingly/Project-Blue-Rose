using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParticleInfo : MonoBehaviour
{
    public float particleSize;
    public float particleSizeVariance;
    public float particleForce;
    public float particleForceVariance;
    public int particleNumberInAGroup;
    public uint particleNumberInAGroupVariance;
    public int particleGroupCount;
    public uint particleGroupCountVariance;
    public float particleLifetime;
    public float rate;

    void Start() { StartCoroutine(DespawnParticle()); }

    IEnumerator DespawnParticle()
    {
        yield return new WaitForSeconds(particleLifetime);
        Destroy(gameObject);
    }
}
