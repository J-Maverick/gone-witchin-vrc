using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PourableBottle : Bottle
{
    [Range(0,1)]
    public float pourThreshold = 0.5f;
    public float maxPourThreshold = 0.5f;
    public float minPourThreshold = -0.2f;
    public Animator pourAnimator;
    public float pourMultiplier = 0.01f;
    public VRCPlayerApi owner = null;

    public Renderer particleRenderer;
    public Material particleMaterial;
    public float pourSpeed = 0f;

    public BottleSync syncObj;

    public VRC_Pickup pickup;
    private float fillAmount = 0.5f;

    private bool triggerFill = false;
    private float flowRate = 0f;
 
    protected override void Start()
    {
        particleMaterial = particleRenderer.material;
        particleMaterial.color = potionColor;
        particleMaterial.SetColor("_EmissionColor", potionColor);
        UpdateLiquidProperties();
        if (owner != null && owner.isLocal) 
        {
            syncObj.SetFill(fillLevel, forceSync: true);
        }
        base.Start();
    }

    public void OnEnable() {
        shaderControl.Start();
        Start();
        Empty();
    }

    public void Empty() {
        fillLevel = 0f;
        syncObj.SetFill(fillLevel, true);
        UpdateShaderFill();
    }

    public void UpdateShaderFill()
    {
        if (shaderControl != null) {
            Debug.Log("Get fucked");
            shaderControl.fillLevel = fillLevel;
        }
        else Debug.Log("FUCK");
    }

    bool CheckPour()
    {
        return transform.up.y < pourThreshold;
    }

    float GetPourSpeed()
    {
        return (pourThreshold - transform.up.y) / (pourThreshold + 1f);
    }

    private void UpdateFill()
    {
        pourThreshold = maxPourThreshold - ((1f - fillLevel) * (maxPourThreshold - minPourThreshold));
        pourSpeed = GetPourSpeed();
        if (pourSpeed < 0.05f) pourSpeed = 0.05f;
        fillLevel -= pourSpeed * pourMultiplier * Time.deltaTime;
        if (fillLevel < 0f)
        {
            fillLevel = 0f;
        }
        UpdateShaderFill();
    }

    public void SetFill(float fill)
    {
        fillLevel = fill;
        pourThreshold = maxPourThreshold - ((1f - fillLevel) * (maxPourThreshold - minPourThreshold));
        if (shaderControl != null) shaderControl.fillLevel = fillLevel;
        if (owner != null && owner.isLocal) syncObj.SetFill(fillLevel, forceSync: true);
    }

    public void AddLiquid(LiquidMaterial newLiquid, float flow)
    {
        if (liquid == null || fillLevel <= 0f)
        {
            liquid = newLiquid;
            UpdateLiquidProperties();
            AddFill(flow);
        }

        else if (liquid == newLiquid)
        {
            AddFill(flow);
        }
    }

    public void UpdateLiquidProperties()
    {
        if (liquid != null)
        {
            potionColor = liquid.color;
            pickup.InteractionText = liquid.name;
            particleMaterial.color = potionColor;
            particleMaterial.SetColor("_EmissionColor", potionColor);
            shaderControl.SetStaticColor(potionColor);
        }
    }

    public void AddFill(float flow)
    {
        if (fillLevel < 1f)
        {
            //fillLevel += fillAmount * flow;
            pourThreshold = maxPourThreshold - ((1f - fillLevel) * (maxPourThreshold - minPourThreshold));
            triggerFill = true;
            flowRate = flow;
        }
        //if (fillLevel > 1f) fillLevel = 1f;
    }

    private void Update()
    {
        TryFill();

        if (fillLevel > 0f && CheckPour())
        {
            UpdateFill();
            if (owner != null && owner.isLocal)
            {
                if (fillLevel == 0) syncObj.SetFill(fillLevel, forceSync: true);
                else syncObj.SetFill(fillLevel);
            }
            //Debug.LogFormat("{0}: transform.up.y: {1}, pourSpeed: {2}", name, transform.up.y, pourSpeed);

            pourAnimator.SetFloat("pourSpeed", pourSpeed);
        }
        else if (pourSpeed > 0f)
        {
            pourSpeed = 0f;
            pourAnimator.SetFloat("pourSpeed", pourSpeed);
        }

    }

    private void TryFill()
    {
        if (triggerFill)
        {
            fillLevel += fillAmount * flowRate * Time.deltaTime;
            if (fillLevel > 1f) fillLevel = 1f;

            UpdateShaderFill();
            syncObj.SetFill(fillLevel);
            triggerFill = false;
        }
    }

    public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
    {
        if (owner != null && owner.isLocal)
        {
            syncObj.SetFill(fillLevel, forceSync: true);
            Networking.SetOwner(requestedOwner, syncObj.gameObject);
        }
        return base.OnOwnershipRequest(requestingPlayer, requestedOwner);
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        owner = player;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (owner != null && owner.isLocal) syncObj.SetFill(fillLevel, forceSync: true);
    }
}
