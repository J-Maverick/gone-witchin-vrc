
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BottleCollision : UdonSharpBehaviour
{
    public AudioClip[] softHitClips;
    public MeshRenderer mesh;
    public Collider meshCollider;
    public GameObject shatterParticles;
    public Rigidbody rigidBody;
    public VRC_Pickup pickup;
    
    public float softHitSpeedLimit = 2f;
    public float softHitVolume = .25f;
    public AudioClip[] mediumHitClips;
    public float mediumHitSpeedLimit = 4f;
    public float mediumHitVolume = .5f;
    public AudioClip[] hardHitClips;
    public float hardHitSpeedLimit = 7f;
    public float hardHitVolume = 1f;
    public AudioClip[] shatterClips;
    public float shatterVolume = 1f;
    public AudioSource audioSource;
    
    private float holdSpeedMultiplier = 3f;

    public void OnCollisionEnter(Collision collision)
    {
        float speed = collision.relativeVelocity.magnitude;
        if (pickup.IsHeld) speed /= holdSpeedMultiplier;

        Debug.LogFormat("{0}: collision velocity magnitude: {1}", name, speed);
        AudioClip[] clips;
        float volume;

        if (speed < softHitSpeedLimit)
        {
            clips = softHitClips;
            volume = softHitVolume + ((speed / softHitSpeedLimit) * (mediumHitVolume - softHitVolume));
        }
        else if (speed < mediumHitSpeedLimit)
        {
            clips = mediumHitClips;
            volume = mediumHitVolume + (((speed - softHitSpeedLimit) / (mediumHitSpeedLimit - softHitSpeedLimit)) * (hardHitVolume - mediumHitVolume));
        }
        else if (speed < hardHitSpeedLimit)
        {
            clips = hardHitClips;
            volume = hardHitVolume;
        }
        else
        {
            clips = shatterClips;
            volume = shatterVolume;
            shatterParticles.SetActive(true);
            mesh.enabled = false;
            meshCollider.enabled = false;
            rigidBody.isKinematic = true;
            pickup.Drop();
            pickup.pickupable = false;
            transform.rotation = Quaternion.identity;
        }


        int randIndex = Random.Range(0, clips.Length);
        audioSource.clip = clips[randIndex];
        audioSource.volume = volume;
        audioSource.Play();
    }
}
