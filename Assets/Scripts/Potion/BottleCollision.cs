
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

    public VRCPlayerApi owner;

    public float softHitSpeedLimit = 2f;
    public float softHitVolume = .25f;
    public float softHitMaxDistance = 8f;
    public AudioClip[] mediumHitClips;
    public float mediumHitSpeedLimit = 4f;
    public float mediumHitVolume = .5f;
    public float mediumHitMaxDistance = 12f;
    public AudioClip[] hardHitClips;
    public float hardHitSpeedLimit = 7f;
    public float hardHitVolume = 1f;
    public float hardHitMaxDistance = 15f;
    public AudioClip[] shatterClips;
    public float shatterVolume = 1f;
    public float shatterMaxDistance = 25f;
    public AudioClip[] soundEffectClips;
    public float soundEffectVolume = 1f;
    public float soundEffectMaxDistance = 25f;
    public AudioSource audioSource;

    public bool isBroken = false;
    public bool brokenSequencePlayed = false;
    
    private float holdSpeedMultiplier = 3f;

    public Bottle bottle = null;
    public PourableBottle refillableBottle = null;
    public bool respawnOnShatter = false;
    public bool refillOnRespawn = false;
    public float respawnTime = 15f;

    public BottleSync syncObj;

    private void Start()
    {
        spawnPosition = spawnTarget.position;
        spawnRotation = spawnTarget.rotation;
        owner = Networking.GetOwner(gameObject);

        if (owner != null && owner.isLocal) syncObj.RandomizeSounds();

        if (!brokenSequencePlayed && isBroken) Shatter();
    }

    public void OnEnable() {
        syncObj.Serialize();
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (owner != null && owner.isLocal)
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
            else if (speed < hardHitSpeedLimit || collision.gameObject.layer == 29)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayHardHit");
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Shatter");
            }
        }
    }

    public void PlaySoftHit()
    {
        AudioClip[] clips = softHitClips;
        audioSource.maxDistance = softHitMaxDistance;
        if (owner != null && owner.isLocal) syncObj.RandomizeSoftHit();
        PlayClip(clips, softHitVolume, syncObj.softHitSoundIndex);
    }

    public void PlayMediumHit()
    {
        AudioClip[] clips = mediumHitClips;
        audioSource.maxDistance = mediumHitMaxDistance;
        if (owner != null && owner.isLocal) syncObj.RandomizeMediumHit();
        PlayClip(clips, mediumHitVolume, syncObj.mediumHitSoundIndex);
    }

    public void PlayHardHit()
    {
        AudioClip[] clips = hardHitClips;
        audioSource.maxDistance = hardHitMaxDistance;
        if (owner != null && owner.isLocal) syncObj.RandomizeHardHit();
        PlayClip(clips, hardHitVolume, syncObj.hardHitSoundIndex);
    }

    public void PlaySoundEffect()
    {
        AudioClip[] clips = soundEffectClips;
        audioSource.maxDistance = soundEffectMaxDistance;
        if (owner != null && owner.isLocal) syncObj.RandomizeSoundEffect();
        PlayClip(clips, soundEffectVolume, syncObj.soundEffectIndex);
    }

    public virtual void Shatter()
    {
        isBroken = true;
        audioSource.maxDistance = shatterMaxDistance;
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

        if (owner != null && owner.isLocal)
        {
            SendCustomEventDelayedSeconds(nameof(DelayedRespawn), respawnTime);
            syncObj.RandomizeShatter();
            syncObj.SetBroken(isBroken);
        }
        PlayClip(clips, volume, syncObj.shatterSoundIndex);
    }

    public void Respawn()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
        mesh.enabled = true;
        meshCollider.enabled = true;
        shatterParticles.SetActive(false);
        pickup.pickupable = true;
        brokenSequencePlayed = false;
        isBroken = false;

        if (owner != null && owner.isLocal)
        {
            syncObj.SetBroken(isBroken);
            if (respawnOnShatter) {
                transform.SetPositionAndRotation(spawnPosition, spawnRotation);
                if (refillOnRespawn && refillableBottle != null) refillableBottle.SetFill(1f);
            }
            else if (bottle != null) {
                bottle.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Bottle.Despawn));
            }
        }
    }

    protected void PlayClip(AudioClip[] clips, float volume, int soundIndex)
    {
        Debug.LogFormat("{0}: Playing clip at volume {1}", name, volume);
        audioSource.clip = clips[soundIndex];
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void DelayedRespawn() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Respawn));
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        owner = player;
        if (owner != null)
        {
            Networking.SetOwner(owner, syncObj.gameObject);
            syncObj.RandomizeSounds();
        }
    }
}
