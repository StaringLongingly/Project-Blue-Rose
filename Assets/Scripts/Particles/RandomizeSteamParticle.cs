using UnityEngine;

public class RandomizeSteamParticle : MonoBehaviour
{
    public MaterialPropertyBlock propertyBlock;
    public Renderer objectRenderer;
    private Color color;
    private float particleLifetime;
    private float startTime;

    void Awake()
    {
        startTime = Time.time;
        particleLifetime = gameObject.GetComponent<SimpleParticleInfo>().particleLifetime; 

        Renderer objectRenderer = gameObject.GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
        objectRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat("_First_Noise_Rotation", Random.Range(0f, 360f));
        propertyBlock.SetFloat("_Second_Noise_Rotation", Random.Range(0f, 360f));
        objectRenderer.SetPropertyBlock(propertyBlock);
    }
    
    void FixedUpdate()
    {        
        float timeFromParticleSpawn = Time.time - startTime;
        float t = timeFromParticleSpawn / particleLifetime;
        t = Mathf.Clamp(t, 0, 1);
        float currentFadePower = EasingFunction.EaseOutQuad(-1, 0.5f, t); 

        Color.RGBToHSV(color, out float H, out float S, out float V);

        propertyBlock.SetFloat("_Alpha", EasingFunction.EaseInQuad(1, 0, t));
        propertyBlock.SetFloat("_Fade_Power", currentFadePower);
        objectRenderer.SetPropertyBlock(propertyBlock);
    }
}
