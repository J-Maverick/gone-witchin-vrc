
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

    [UdonSynced] public bool isBroken = false;
    public bool brokenSequencePlayed = false;
    
    private float holdSpeedMultiplier = 3f;

    public void OnCollisionEnter(Collision collision)
    {
        float speed = collision.relativeVelocity.magnitude;
        if (pickup.IsHeld) speed /= holdSpeedMultiplier;

        Debug.LogFormat("{0}: collision velocity magnitude: {1}", name, speed);

        // Determine which audio clips to play based on impact speed, or shatter
        if (speed < softHitSpeedLimit)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlaySoftHit");
        }
        else if (speed < mediumHitSpeedLimit)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayMediumHit");
        }
        else if (speed < hardHitSpeedLimit)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayHardHit");
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Shatter");
        }
    }

    public void PlaySoftHit()
    {
        AudioClip[] clips = softHitClips;
        audioSource.maxDistance = 8f;
        PlayClip(clips, softHitVolume);
    }

    public void PlayMediumHit()
    {
        AudioClip[] clips = mediumHitClips;
        audioSource.maxDistance = 12f;
        PlayClip(clips, mediumHitVolume);
    }

    public void PlayHardHit()
    {
        AudioClip[] clips = hardHitClips;
        audioSource.maxDistance = 15f;
        PlayClip(clips, hardHitVolume);
    }

    public void Shatter()
    {
        isBroken = true;
        audioSource.maxDistance = 25f;
        AudioClip[] clips = shatterClips;
        float volume = shatterVolume;
        shatterParticles.SetActive(true);
        mesh.enabled = false;
        meshCollider.enabled = false;
        rigidBody.isKinematic = true;
        pickup.Drop();
        pickup.pickupable = false;
        transform.rotation = Quaternion.identity;
        brokenSequencePlayed = true;

        PlayClip(clips, volume);
    }

    private void PlayClip(AudioClip[] clips, float volume)
    {
        int randIndex = Random.Range(0, clips.Length);
        audioSource.clip = clips[randIndex];
        audioSource.volume = volume;
        audioSource.Play();
    }

    private void Update()
    {
        if (!brokenSequencePlayed && isBroken) Shatter();
    }
}
