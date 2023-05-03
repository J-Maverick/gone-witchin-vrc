
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BottleSync : UdonSharpBehaviour
{
    [UdonSynced] public float fillLevel = 0.0f;
    [UdonSynced] public int softHitSoundIndex = 0;
    [UdonSynced] public int mediumHitSoundIndex = 0;
    [UdonSynced] public int hardHitSoundIndex = 0;
    [UdonSynced] public int shatterSoundIndex = 0;
    [UdonSynced] public int soundEffectIndex = 0;
    [UdonSynced] public bool isBroken = false;

    public BottleCollision bottleCollision;
    public PourableBottle pourableBottle = null;

    public float updateTimer = 0f;
    public float nextUpdate = 0f;

    private int lateJoinRetries = 10;
    private float lateJoinRetryTime = 1f;
    private float lateJoinNextTry = 0f;
    private float lateJoinRetryTimer = 0f;
    private int lateJoinRetryCount = 0;
    private bool lateJoinRetry = false;

    public void RandomizeSounds()
    {
        softHitSoundIndex = Random.Range(0, bottleCollision.softHitClips.Length);
        mediumHitSoundIndex = Random.Range(0, bottleCollision.mediumHitClips.Length);
        hardHitSoundIndex = Random.Range(0, bottleCollision.hardHitClips.Length);
        shatterSoundIndex = Random.Range(0, bottleCollision.shatterClips.Length);
        RequestSerialization();
    }

    public void RandomizeSoftHit()
    {
        mediumHitSoundIndex = Random.Range(0, bottleCollision.mediumHitClips.Length);
        RequestSerialization();
    }

    public void RandomizeMediumHit()
    {
        softHitSoundIndex = Random.Range(0, bottleCollision.softHitClips.Length);
        RequestSerialization();
    }

    public void RandomizeHardHit()
    {
        hardHitSoundIndex = Random.Range(0, bottleCollision.hardHitClips.Length);
        RequestSerialization();
    }

    public void RandomizeShatter()
    {
        shatterSoundIndex = Random.Range(0, bottleCollision.shatterClips.Length);
        RequestSerialization();
    }

    public void RandomizeSoundEffect()
    {
        soundEffectIndex = Random.Range(0, bottleCollision.soundEffectClips.Length);
        RequestSerialization();
    }

    public void SetBroken(bool newVal)
    {
        isBroken = newVal;
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        bottleCollision.isBroken = isBroken;
        if (pourableBottle != null)
        {
            pourableBottle.fillLevel = fillLevel;
            pourableBottle.UpdateShaderFill();
        }
    }

    public override void OnPreSerialization()
    {
        if (pourableBottle != null)
        {
            fillLevel = pourableBottle.fillLevel;
        }
        if (bottleCollision != null)
        {
            isBroken = bottleCollision.isBroken;
        }
    }

    public void SetFill(float fill, bool forceSync=false)
    {
        fillLevel = fill;
        if (forceSync) RequestSerialization();
        else FrequencySerialization();
    }

    public void FrequencySerialization(float freq = 5f)
    {
        if (updateTimer >= nextUpdate)
        {
            RequestSerialization();
            nextUpdate = updateTimer + (1f / freq);
        }
        updateTimer += Time.deltaTime;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.GetOwner(gameObject).isLocal)
        {
            RequestSerialization();
            lateJoinRetryTimer = Time.realtimeSinceStartup;
            lateJoinNextTry = Time.realtimeSinceStartup + lateJoinRetryTime + Random.Range(0f, lateJoinRetryTime);
            lateJoinRetryCount = 0;
            lateJoinRetry = true;
        }
    }

    public void Update()
    {
        if (lateJoinRetry)
        {
            if (lateJoinRetryTimer >= lateJoinNextTry)
            {
                Debug.LogFormat("{0}: Late joiner serialization retrying", name);
                lateJoinRetryCount++;
                lateJoinNextTry += lateJoinRetryTime;
                RequestSerialization();
            }
            lateJoinRetryTimer += Time.deltaTime;
            if (lateJoinRetryCount > lateJoinRetries) lateJoinRetry = false;
        }
    }
}
