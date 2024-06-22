using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSoundWhenMoving : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource audioSource;
    public float velocity;
    public float minVelocity = 0.2f;
    public float maxVelocity = 20f;
    public float pitchScalar;
    public float volumeScalar;

    // Start is called before the first frame update
    void Start()
    {
       rb = gameObject.GetComponent<Rigidbody>(); 
       audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = rb.velocity.magnitude;

        if (velocity < minVelocity) audioSource.Stop();
        else if (audioSource.isPlaying == false) audioSource.Play();

        if (velocity > maxVelocity) velocity = maxVelocity;

        float velocityScaledForPitch = Mathf.Clamp(velocity * pitchScalar, 0, maxVelocity);
        float velocityScaledForVolume = Mathf.Clamp(velocity * volumeScalar, 0, maxVelocity);

        audioSource.pitch = Remap(velocityScaledForPitch, 0f, maxVelocity, 1f, 3f);
        audioSource.volume = Remap(velocityScaledForVolume, 0f, maxVelocity, 0f, 1f); 
    }

    float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}