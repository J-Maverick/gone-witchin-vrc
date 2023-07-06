
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

    [UdonSynced, FieldChangeCallback(nameof(BottleID))] 
    private int _bottleID = 2;

    public int BottleID 
    {
        set 
        {
            if (value != _bottleID) {
                Debug.LogFormat("{0}: Bottle ID updated to {1}", name, value);
                _bottleID = value;
                UpdateBottleMesh();
            }
        }
        get => _bottleID;
    }

    public BottleCollision bottleCollision;
    public PourableBottle pourableBottle = null;

    public float updateTimer = 0f;
    public float nextUpdate = 0f;

    public float intervalTime = 3f;
    private int nJoinSyncs = 10;
    public int joinSyncCounter = 0;

    public BottleDataList bottleDataList;
    public MeshFilter filter;
    public PotionWobble wobble;

    public void SetBottleType(int ID) {
        Debug.LogFormat("{0}: Setting bottle ID to {1}", name, ID);
        BottleID = ID;
        RequestSerialization();
    }

    public void UpdateBottleMesh() {
        Debug.LogFormat("{0}: Attempting to update bottle data", name);
        if (bottleDataList != null) {
            Debug.LogFormat("{0}: Getting bottle data from list", name);
            BottleData bottleData = bottleDataList.GetBottleByID(_bottleID);
            if (bottleData != null) {
                filter.mesh = bottleData.mesh;
                wobble.minFill = bottleData.minFill;
                wobble.maxFill = bottleData.maxFill;
                wobble.UpdateFillLevel();
                Debug.LogFormat("{0}: Updated bottle data", name);
            }
        }
    }

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

    public void JoinSync() {
        if (joinSyncCounter < nJoinSyncs) {
            SendCustomEventDelayedSeconds("JoinSync", intervalTime);
            RequestSerialization();
            joinSyncCounter++;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        joinSyncCounter = 0;
        JoinSync();
    }
}
