
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Persistence;

public class FishingPole : UdonSharpBehaviour
{
    public GameObject lure;
    public GameObject hook;
    public GameObject[] ownedObjects;
    public VRCPickup pickup;
    public VRCPlayerApi assignedPlayer = null;
    public FishForce fishForce;

    public Text lureSpringText = null;
    public Text lureMassText = null;
    public Text lureDragText = null;
    public Text lureForceText = null;
    public Text fishForceText = null;

    public SpringJoint lureJoint;
    public Rigidbody lureRigidBody;
    public SpringJoint hookJoint;
    public Rigidbody hookRigidBody;

    public RigidBodyCorrector[] rigidBodyCorrectors;

    public RodUpgrade rodLevelParams = null;

    public float addSpringRatio = 0.05f;
    private float distanceRatio = 0.001f;

    public bool isHeld;

    private float staticTension = 1000f;
    private float staticMass = 5f; 
    private float staticDrag = 2f;
    private float staticDistance = 1f;
    private float staticHookTension = 1000f;
    private float staticHookMass = 200f;
    private float staticHookDrag = 2f;
    private float staticHookDistance = 0.5f;

    private bool casting = false;
    public bool casted = false;
    public bool inWater = false;
    public bool fishOn = false;

    public Water water = null;

    public float castDistance = 0f;

    public float minRunoutTime = 0.75f;
    public float maxRunoutTime = 2f;
    public float runoutTime = 0f;
    public float runoutTimer = 0f;
    public float runoutOffsetDistance = 5f;

    public bool desktopReeling = false;
    public bool reeling = true;
    public float reelingTimer = 0f;
    public ReelAngleAccumulator reelAngleAccumulator = null;
    public RandomAudioHandler bobberAudio;
    public RandomAudioHandler reelAudio;
    public float maxReelVolume = 0.6f;
    public float minReelVolume = 0.1f;

    private float maxDistance = 50f;
    public bool localPlayerClose = false;
    public int frameOffset = 0;

    public bool runningOut = false;

    public bool reelHapticsEnabled = true;

    public void Start()
    {
        SetLureText();
        CheckLocalPlayerClose();
        frameOffset = Random.Range(0, 100);
    }

    public void CheckLocalPlayerClose()
    {
        if (Networking.LocalPlayer != null)
        {
            float distance = Vector3.Distance(Networking.LocalPlayer.GetPosition(), transform.position);
            localPlayerClose = distance < maxDistance;
        }
    }

    public override void OnPickup()
    {
        isHeld = true;
        hookRigidBody.WakeUp();
        lureRigidBody.WakeUp();
    }

    public override void OnDrop()
    {
        isHeld = false;
        desktopReeling = false;
    }

    public override void OnPickupUseDown()
    {
        if (fishOn) {}
        else if (casting || casted || inWater)
        {
            if (Networking.LocalPlayer.IsUserInVR()) {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetLure));
            }
        }
        else SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(CastingEvent));
    }

    public override void OnPickupUseUp()
    {
        if (casting)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(CastEvent));
        }
    }

    public void CatchVibration(float fishSize) {
        float duration = 1.5f + fishSize;
        float amplitude = 0.5f + (fishSize / 2f);
        pickup.GenerateHapticEvent(duration:duration, amplitude:amplitude, frequency:30f);
    }

    public void TickVibration() {
        if (!reelHapticsEnabled) return;
        if (fishOn) {
            float lureMagnitude = lureJoint.currentForce.magnitude;
            float fishMagnitude = fishForce.fishForce.magnitude;
            float fishStatsAdd = fishForce.fish != null ? (Mathf.Log(2f * fishForce.fish.weight) * .04f) + (fishForce.fish.size * 0.1f) : 0f;
            float amplitudeAdd = fishMagnitude > 0f ? 0.1f * lureMagnitude / fishMagnitude : 0.05f + fishStatsAdd;
            float amplitude =  0.1f + fishStatsAdd + amplitudeAdd;
            if (pickup.currentPlayer != null) {
                pickup.currentPlayer.PlayHapticEventInHand(pickup.currentHand, duration:0.005f, amplitude:amplitude, frequency:160f);
            }
            if (reelAngleAccumulator.pickup.currentPlayer != null) {
                reelAngleAccumulator.pickup.currentPlayer.PlayHapticEventInHand(reelAngleAccumulator.pickup.currentHand, duration:0.005f, amplitude:amplitude, frequency:160f);
            }
        }
        else {
            if (pickup.currentPlayer != null) {
                pickup.currentPlayer.PlayHapticEventInHand(pickup.currentHand, duration:0.005f, amplitude:0.1f, frequency:160f);
            }
            if (reelAngleAccumulator.pickup.currentPlayer != null) {
                reelAngleAccumulator.pickup.currentPlayer.PlayHapticEventInHand(reelAngleAccumulator.pickup.currentHand, duration:0.005f, amplitude:0.1f, frequency:160f);
            }
        }
    }

    public float GetCastDistance()
    {
        if (casted)
        {
            castDistance = (lure.transform.position - transform.position).magnitude;
        }

        return castDistance;
    }

    public void CastingEvent()
    {
        Debug.Log("Casting Event Triggered");
        lureRigidBody.drag = 0.1f;
        hookRigidBody.drag = 0.1f;
        casting = true;
    }

    public void CastEvent()
    {
        Debug.Log("Cast Event Triggered");
        lureJoint.spring = rodLevelParams.castTension;
        SetLureText();
        casted = true;
    }

    public void SetWater(Water newWater)
    {
        water = newWater;
    }

    public void SplashDown(Water splashDownWater)
    {
        if (water != splashDownWater) SetWater(splashDownWater);
        if (casted)
        {
            bobberAudio.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(bobberAudio.PlaySlotZero));
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetSplashDownParams));
        }
    }

    public void SetSplashDownParams()
    {
        Debug.LogFormat("{0}: Setting Splash Down Params", name);
        addSpringRatio = rodLevelParams.castSpringRatio;
        distanceRatio = rodLevelParams.castDistanceRatio;
        lureJoint.spring = rodLevelParams.castTension;
        lureRigidBody.mass = rodLevelParams.castWeight;
        lureRigidBody.drag = rodLevelParams.castDrag;
        castDistance = (lure.transform.position - transform.position).magnitude;
        lureJoint.minDistance = castDistance;

        // lureRigidBody.constraints = RigidbodyConstraints.FreezePositionY;
        hookJoint.spring = staticHookTension;
        hookRigidBody.mass = staticHookMass;
        hookRigidBody.drag = staticHookDrag;
        hookJoint.minDistance = staticHookDistance;
        inWater = true;
        casting = false;
        casted = false;
        SetLureText();
    }

    public void FishOn(float fishSize)
    {
        if (inWater && !fishOn)
        {
            runoutTime = minRunoutTime + (fishSize * (maxRunoutTime - minRunoutTime));
            CatchVibration(fishSize);
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetFishOnParams));
        }
    }

    public void SetFishOnParams()
    {
        Debug.LogFormat("{0}: Setting Fish On Params", name);
        lureRigidBody.drag = rodLevelParams.catchDrag;
        lureRigidBody.mass = rodLevelParams.catchWeight;
        lureJoint.spring = rodLevelParams.catchTension;
        lureJoint.minDistance = (lure.transform.position - transform.position).magnitude;
        addSpringRatio = rodLevelParams.fishOnSpringRatio;
        distanceRatio = rodLevelParams.fishOnDistanceRatio;
        SetLureText();
        runoutTimer = 0f;
        reelingTimer = 0f;
        reeling = true;
        fishOn = true;
    }

    public void RunOut()
    {
        if (runoutTimer < 0.5f) {
            if (!reelAudio.audioSource.isPlaying) {
                if (!(fishForce.fish.fishData.tags.Length > 0 && fishForce.fish.fishData.tags[0] == FishTag.Recipe)) {
                    reelAudio.slotZeroVolume = Mathf.Lerp(minReelVolume, maxReelVolume, fishForce.fish.weight / 100f);
                    reelAudio.PlaySlotZero();
                }
            }
        }
        if (runoutTimer < runoutTime)
        {
            runningOut = true;
            float offsetDistance = (lure.transform.position - transform.position).magnitude - runoutOffsetDistance;
            if (offsetDistance > lureJoint.minDistance) lureJoint.minDistance = offsetDistance;
        }
        else {
            runningOut = false;
        }
        runoutTimer += Time.fixedDeltaTime;
    }

    public void FishOff()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetFishOffParams));
        SplashDown(water);      
    }

    public void SetFishOffParams()
    {
        casted = true;
        fishOn = false;
    }

    public void UnlockLure()
    {
        if (!inWater) return;
        Debug.LogFormat("{0}: Unlocking Lure", name);
        lureRigidBody.constraints = RigidbodyConstraints.None;
        lureRigidBody.mass /= 10f;
        lureRigidBody.drag /= 2f;
        lureRigidBody.angularDrag = 1f;
        if (fishForce.fish != null) hookRigidBody.mass *= fishForce.fish.weight;
        hookJoint.spring *= 2f;
        hookJoint.minDistance = 0.1f;
        lureJoint.minDistance = (lure.transform.position - transform.position).magnitude - 0.25f;
        lureJoint.spring *= 10f;
    }

    public void ResetLureSync()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetLure));
    }

    public void ResetLure()
    {
        Debug.LogFormat("{0}: Resetting Lure", name);
        lureRigidBody.constraints = RigidbodyConstraints.None;
        lureJoint.spring = staticTension;
        lureRigidBody.mass = staticMass;
        lureRigidBody.drag = staticDrag;
        lureJoint.minDistance = staticDistance;
        lureRigidBody.angularDrag = 0f;
        hookJoint.spring = staticHookTension;
        hookRigidBody.mass = staticHookMass;
        hookRigidBody.drag = staticHookDrag;
        hookJoint.minDistance = staticHookDistance;
        casting = false;
        inWater = false;
        casted = false;
        fishOn = false;
        SetLureText();
    }

    public void AddSpring(float spring)
    {
        reeling = true;
        reelingTimer = 0f;
        if (inWater)
        {
            if (lureJoint.spring < rodLevelParams.maxSpring)
            {
                lureJoint.spring += addSpringRatio * spring;
            }
            if (lureJoint.minDistance != 0f)
            {
                float distance = lureJoint.minDistance - (distanceRatio * spring);
                if (distance < 0f) distance = 0f;
                lureJoint.minDistance = distance;
            }
            SetLureText();
        }
    }

    private void SetLureText()
    {
        if (lureSpringText != null)
        {
            lureSpringText.text = string.Format("Lure Spring: {0:0.##}", lureJoint.spring);
        }
        if (lureMassText != null)
        {
            lureMassText.text = string.Format("Lure Mass: {0:0.##}", lureRigidBody.mass);
        }
        if (lureDragText != null)
        {
            lureDragText.text = string.Format("Lure Drag: {0:0.##}", lureRigidBody.drag);
        }
        if (lureForceText != null) {
            lureForceText.text = string.Format("Lure Joint Force: {0:0.##}", lureJoint.currentForce.magnitude);
        }
        if (fishForceText != null) {
            fishForceText.text = string.Format("Fish Force: {0:0.##}", fishForce.fishForce.magnitude);
        }
    }

    public void Update()
    {
        if (reelingTimer > rodLevelParams.reelingInactiveTime) reeling = false;
        else reelingTimer += Time.deltaTime;
        if (((Time.frameCount + frameOffset) % 100) == 0) CheckLocalPlayerClose();
        if (!Networking.LocalPlayer.IsUserInVR() && isHeld) {
            if (!fishOn && Input.GetMouseButtonDown(2)) {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetLure));
            }
            if (Input.GetMouseButton(0) && (casted || inWater)) {
                desktopReeling = true;
            }
            else {
                desktopReeling = false;
            }
        }
    }



    public void Teleport(Transform position)
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetFishOffParams));
        ResetLureSync();
        Debug.LogFormat("{0}: Teleporting to {1}", name, position);
        VRCObjectSync sync = GetComponent<VRCObjectSync>();
        VRCObjectSync lureSync = lure.GetComponent<VRCObjectSync>();
        VRCObjectSync hookSync = hook.GetComponent<VRCObjectSync>();
        if (sync == null || lureSync == null || hookSync == null) return;
        lureSync.TeleportTo(position);
        hookSync.TeleportTo(position);
        sync.TeleportTo(position);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetRigidBodies));
    }

    public void ResetRigidBodies() {
        foreach (RigidBodyCorrector rbc in rigidBodyCorrectors) {
            rbc.ResetRigidbody();
            lureRigidBody.velocity = Vector3.zero;
            lureRigidBody.angularVelocity = Vector3.zero;
            hookRigidBody.velocity = Vector3.zero;
            hookRigidBody.angularVelocity = Vector3.zero;
        }
    }

    public void SetPoleFlip()
    {
        bool poleFlipped = PlayerData.GetBool(Networking.LocalPlayer, DataKeys.PoleFlipped);
        if (poleFlipped)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            lure.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            lure.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    /* =================================================
      
                          NETWORKING
                          
      ================================================= */


    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (Networking.GetOwner(gameObject) == player)
        {
            EnablePickup();
            SetPoleFlip();
        }
    }
    
    public void EnablePickup() {
        if (Networking.GetOwner(gameObject).isLocal) {
            Debug.LogFormat("{0}: Enable Pickup", name);
            pickup.pickupable = true;
            reelAngleAccumulator.pickup.pickupable = true;
        }
        else {
            Debug.LogFormat("{0}: Disable Pickup", name);
            pickup.pickupable = false;
            reelAngleAccumulator.pickup.pickupable = false;
        }
    }
}
