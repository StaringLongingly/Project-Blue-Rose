using UnityEngine;

public class ObjectExploder : MonoBehaviour
{
    private Rigidbody rb;
    public float velocityThreshold;
    private SimpleParticleSystem simpleParticleSystem;
    private MeshDestroy meshDestroy;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();        
        simpleParticleSystem = gameObject.GetComponent<SimpleParticleSystem>();
        meshDestroy = gameObject.GetComponent<MeshDestroy>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (rb.velocity.magnitude > velocityThreshold)
        {
            if (collider.gameObject.TryGetComponent<ObjectExploder>(out var objectExploder)) ExplodeObject();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > velocityThreshold)
        {
            if (collision.gameObject.TryGetComponent<ObjectExploder>(out var objectExploder)) ExplodeObject();
        }
    }

    [ContextMenu("Explode Object")]
    public void ExplodeObject()
    {
        simpleParticleSystem.SpawnAllParticleTypes(); 
        meshDestroy.DestroyMesh();
        Destroy(gameObject);
    }
}