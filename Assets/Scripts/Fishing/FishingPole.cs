
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class FishingPole : UdonSharpBehaviour
{
    public GameObject lure;
    public GameObject hook;
    public GameObject[] ownedObjects;

    public FishForce fishForce;

    public Text lureSpringText = null;
    public Text lureMassText = null;
    public Text lureDragText = null;

    public SpringJoint lureJoint;
    private Rigidbody lureRigidBody;
    private SpringJoint hookJoint;
    private Rigidbody hookRigidBody;

    public float castTension = 1f;
    public float castWeight = 1f;
    public float castDrag = 5f;

    public float catchTension = 1000f;
    public float catchWeight = 100f;
    public float catchDrag = 10f;

    public float fishOnSpringRatio = 0.05f;
    public float castSpringRatio = 0.001f;
    public float castDistanceRatio = 0.003f;
    public float fishOnDistanceRatio = 0.001f;
    public float maxSpring = 2500f;

    private float addSpringRatio = 0.05f;
    private float distanceRatio = 0.001f;

    public bool isHeld;

    private float staticTension;
    private float staticMass;
    private float staticDrag;
    private float staticDistance;

    private float staticHookTension;
    private float staticHookMass;
    private float staticHookDrag;
    private float staticHookDistance;

    private bool casting = false;
    private bool casted = false;
    public bool inWater = false;
    public bool fishOn = false;

    public Water water = null;
    public Water lakeWater;
    public Water caveWater;

    public float castDistance = 0f;

    public float minRunoutTime = 0.75f;
    public float maxRunoutTime = 2f;
    public float runoutTime = 0f;
    public float runoutTimer = 0f;
    public float runoutOffsetDistance = 5f;

    public bool reeling = true;
    public float reelingTimer = 0f;
    public float reelingInactiveTime = 10f;

    void Start()
    {
        lureJoint = lure.GetComponent<SpringJoint>();
        lureRigidBody = lure.GetComponent<Rigidbody>();
        staticTension = lureJoint.spring;
        staticMass = lureRigidBody.mass;
        staticDrag = lureRigidBody.drag;
        staticDistance = lureJoint.minDistance;

        hookJoint = hook.GetComponent<SpringJoint>();
        hookRigidBody = hook.GetComponent<Rigidbody>();
        staticHookTension = hookJoint.spring;
        staticHookMass = hookRigidBody.mass;
        staticHookDrag = hookRigidBody.drag;
        staticHookDistance = hookJoint.minDistance;

        SetLureText();
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
    }

    public override void OnPickupUseDown()
    {
        if (fishOn) SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FishOff));
        else if (casting || casted || inWater)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetLure));
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
        casting = true;
    }

    public void CastEvent()
    {
        Debug.Log("Cast Event Triggered");
        lureJoint.spring = castTension;
        SetLureText();
        casted = true;
    }

    public void SetWater(Water newWater)
    {
        if (newWater == lakeWater) SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetLakeWater));
        else if (newWater == caveWater) SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetCaveWater));
    }

    public void SetLakeWater()
    {
        water = lakeWater;
    }

    public void SetCaveWater()
    {
        water = caveWater;
    }
    public void SplashDown(Water splashDownWater)
    {
        if (water != splashDownWater) SetWater(splashDownWater);
        if (casted)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetSplashDownParams));
        }
    }

    public void SetSplashDownParams()
    {
        addSpringRatio = castSpringRatio;
        distanceRatio = castDistanceRatio;
        lureJoint.spring = castTension;
        lureRigidBody.mass = castWeight;
        lureRigidBody.drag = castDrag;
        castDistance = (lure.transform.position - transform.position).magnitude;
        lureJoint.minDistance = castDistance;

        lureRigidBody.constraints = RigidbodyConstraints.FreezePositionY;
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
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetFishOnParams));
        }
    }

    public void SetFishOnParams()
    {
        lureRigidBody.drag = catchDrag;
        lureRigidBody.mass = catchWeight;
        lureJoint.spring = catchTension;
        lureJoint.minDistance = (lure.transform.position - transform.position).magnitude;
        addSpringRatio = fishOnSpringRatio;
        distanceRatio = fishOnDistanceRatio;
        SetLureText();
        runoutTimer = 0f;
        reelingTimer = 0f;
        reeling = true;
        fishOn = true;
    }

    public void RunOut()
    {
        if (runoutTimer < runoutTime)
        {
            float offsetDistance = (lure.transform.position - transform.position).magnitude - runoutOffsetDistance;
            if (offsetDistance > lureJoint.minDistance) lureJoint.minDistance = offsetDistance;
            // This is where we would play the runout sound I think
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
        if (lureRigidBody.constraints != RigidbodyConstraints.None)
        {
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
    }

    public void ResetLureSync()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetLure));
    }

    public void ResetLure()
    {
        Debug.Log("Reset Lure Event Triggered");
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
            if (lureJoint.spring < maxSpring)
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
    }

    private void Update()
    {
        if (reelingTimer > reelingInactiveTime) reeling = false;
        else reelingTimer += Time.deltaTime;
    }

    /* =================================================
      
                          NETWORKING
                          
      ================================================= */

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            foreach (GameObject obj in ownedObjects) Networking.SetOwner(player, obj);
            if (fishForce.fish != null) Networking.SetOwner(player, fishForce.fish.gameObject);
            lureRigidBody.WakeUp();
            hookRigidBody.WakeUp();
            //TODO add all required objects -- or make ownership transfer cascade
        }
    }
}
