
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BottleCollision : UdonSharpBehaviour
{
    public Transform spawnTarget;
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;
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

    public float respawnTime = 15f;
    private float respawnTimer = 0f;
    private bool respawning = false;

    public PourableBottle refillableBottle = null;
    public bool refillOnRespawn = false;

    private void Start()
    {
        spawnPosition = spawnTarget.position;
        spawnRotation = spawnTarget.rotation;
    }

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

    public virtual void Shatter()
    {
        isBroken = true;
        audioSource.maxDistance = 25f;
        AudioClip[] clips = shatterClips;
        float volume = shatterVolume;
        shatterParticles.SetActive(true);
        mesh.enabled = false;
        meshCollider.enabled = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        pickup.Drop();
        pickup.pickupable = false;
        transform.rotation = Quaternion.identity;
        brokenSequencePlayed = true;

        PlayClip(clips, volume);
        TriggerRespawn();
    }

    private void TriggerRespawn()
    {
        respawning = true;
    }

    public void Respawn()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        mesh.enabled = true;
        meshCollider.enabled = true;
        shatterParticles.SetActive(false);
        pickup.pickupable = true;
        brokenSequencePlayed = false;
        isBroken = false;

        if (refillOnRespawn && refillableBottle != null) refillableBottle.SetFill(1f);
    }

    private void PlayClip(AudioClip[] clips, float volume)
    {
        int randIndex = Random.Range(0, clips.Length);
        audioSource.clip = clips[randIndex];
        audioSource.volume = volume;
        audioSource.Play();
    }

    private void UpdateRespawn()
    {
        if (respawning)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnTime)
            {
                Respawn();
                respawnTimer = 0f;
                respawning = false;
            }
        }
    }

    private void Update()
    {
        if (!brokenSequencePlayed && isBroken) Shatter();
        UpdateRespawn();
    }
}
