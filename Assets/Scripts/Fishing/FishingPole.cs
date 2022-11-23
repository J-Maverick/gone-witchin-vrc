
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class FishingPole : UdonSharpBehaviour
{
    public GameObject lure;
    public GameObject hook;

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
    }

    public override void OnDrop()
    {
        isHeld = false;
    }
    
    public override void OnPickupUseDown()
    {
        if (fishOn) FishOff();
        else if (casting || casted || inWater)
        {
            ResetLure();
        }
        else casting = true;
    }
    public override void OnPickupUseUp()
    {
        if (casting)
        {
            lureJoint.spring = castTension;
            SetLureText();
            casted = true;
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

    public void SplashDown(Water splashDownWater)
    {
        if (water != splashDownWater) water = splashDownWater;
        if (casted)
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
    }

    public void FishOn(float fishSize)
    {
        if (inWater && !fishOn)
        {
            lureRigidBody.drag = catchDrag;
            lureRigidBody.mass = catchWeight;
            lureJoint.spring = catchTension;
            lureJoint.minDistance = (lure.transform.position - transform.position).magnitude;
            addSpringRatio = fishOnSpringRatio;
            distanceRatio = fishOnDistanceRatio;
            SetLureText();
            runoutTime = minRunoutTime + (fishSize * (maxRunoutTime - minRunoutTime));
            runoutTimer = 0f;
            reelingTimer = 0f;
            reeling = true;
            fishOn = true;
        }
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
        casted = true;
        fishOn = false;
        SplashDown(water);
    }

    public void UnlockLure()
    {
        if (fishOn && lureRigidBody.constraints != RigidbodyConstraints.None)
        {
            lureRigidBody.constraints = RigidbodyConstraints.None;
            lureRigidBody.mass /= 10f;
            lureRigidBody.drag /= 2f;
            lureRigidBody.angularDrag = 1f;
            hookRigidBody.mass *= fishForce.fish.weight;
            hookJoint.spring *= 2f;
            hookJoint.minDistance = 0.1f;
            lureJoint.minDistance = (lure.transform.position - transform.position).magnitude - 0.25f;
            lureJoint.spring *= 10f;
        }
    }

    public void ResetLure()
    {
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
        if (player.isLocal) Networking.SetOwner(player, lure);
    }
}
