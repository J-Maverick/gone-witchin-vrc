
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
    [UdonSynced] public bool isBroken = false;

    public BottleCollision bottleCollision;
    public PourableBottle pourableBottle = null;

    public float updateTimer = 0f;
    public float nextUpdate = 0f;

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
}
