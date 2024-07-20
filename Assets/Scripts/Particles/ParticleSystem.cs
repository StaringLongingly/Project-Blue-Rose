using System.Collections;
using UnityEngine;

public class SimpleParticleSystem : MonoBehaviour
{
    public GameObject[] particleTypes;
    public GameObject[][] particles;
    public GameObject particleParent; 

    [ContextMenu("Spawn Particles")]
    public void SpawnAllParticleTypes()
    {
        particles = new GameObject[particleTypes.Length][];
        particleParent = GameObject.Find("Particles");
        for (int i = 0; i < particleTypes.Length; i++) { StartCoroutine(SpawnOneParticleType(i)); }
    }

    [ContextMenu("Despawn Particles")]
    public void DespawnAllParticles()
    {
        foreach (GameObject[] particleGroup in particles)
        {
            if (particleGroup == null) continue;
            
            foreach (GameObject particle in particleGroup)
            {
                if (particle == null) continue;
                DestroyImmediate(particle);
            } 
        }
    }

    private IEnumerator SpawnOneParticleType(int particleTypeIndex)
    {
        SimpleParticleInfo simpleParticleInfo = particleTypes[particleTypeIndex].GetComponent<SimpleParticleInfo>(); 
        if (simpleParticleInfo == null)
        {
            Debug.LogError("Prefab does not have SimpleParticleInfo script attached");
            yield return 0;
        }

        float particleSize = simpleParticleInfo.particleSize;
        float particleSizeVariance = simpleParticleInfo.particleSizeVariance;
        float particleSizeVaried = particleSize + Random.Range(-Mathf.Abs(particleSizeVariance), Mathf.Abs(particleSizeVariance));

        float particleForce = simpleParticleInfo.particleForce;
        float particleForceVariance = simpleParticleInfo.particleForceVariance;
        float particleForceVaried = particleForce + Random.Range(-Mathf.Abs(particleForceVariance), Mathf.Abs(particleForceVariance));

        float rate = simpleParticleInfo.rate;

        int particleNumberInAGroup = simpleParticleInfo.particleNumberInAGroup;
        uint particleNumberInAGroupVariance = simpleParticleInfo.particleNumberInAGroupVariance;
        int particleNumberInAGroupVaried = particleNumberInAGroup + Random.Range(-(int)particleNumberInAGroupVariance, (int)particleNumberInAGroup + 1);

        int particleGroupCount = simpleParticleInfo.particleGroupCount;
        uint particleGroupCountVariance = simpleParticleInfo.particleGroupCountVariance;
        int particleGroupCountVaried = particleGroupCount + Random.Range(-(int)particleGroupCountVariance, (int)particleGroupCountVariance + 1);

        particles[particleTypeIndex] = new GameObject[(particleNumberInAGroup + particleNumberInAGroupVariance) * (particleGroupCount + particleGroupCountVariance + 1)];

        //Outer Loop for Groups, Inner Loop for particles in a Group
        for (int currentParticleGroupIndex = 0; currentParticleGroupIndex < particleGroupCountVaried; currentParticleGroupIndex++)
        {
            for (int currentParticleIndex = currentParticleGroupIndex * particleNumberInAGroup; currentParticleIndex < (currentParticleGroupIndex + 1) * particleNumberInAGroupVaried; currentParticleIndex++)
            {
                particles[particleTypeIndex][currentParticleIndex] = Instantiate(
                    particleTypes[particleTypeIndex],
                    gameObject.transform.position,
                    Quaternion.identity,
                    particleParent.transform                     
                );

                TrailRenderer trailRenderer = particles[particleTypeIndex][currentParticleIndex].GetComponent<TrailRenderer>();
                if (trailRenderer != null) trailRenderer.widthMultiplier *= particleSizeVaried * 0.5f;
                particles[particleTypeIndex][currentParticleIndex].transform.localScale = Vector3.one * particleSizeVaried;
            
                Rigidbody rb = particles[particleTypeIndex][currentParticleIndex].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Random.onUnitSphere * particleForceVaried);
                }
            }

            yield return new WaitForSeconds(rate);
        }
    }

}
