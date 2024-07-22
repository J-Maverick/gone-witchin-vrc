using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CauldronPour : UdonSharpBehaviour
{
    public Cauldron cauldron;
    public GemIndicator indicator;
    public BottleSnap bottleSnap;
    public Renderer particleRenderer;
    public Material particleMaterial;
    public Animator particleAnimator;

    public Lever lever;

    public float maxFlow = 0.6f;
    public float flow = 0f;
    private float pourMultiplier = 0.2f;

    public float intervalTime = 3f;
    private int nJoinSyncs = 10;
    public int joinSyncCounter = 0;

    LiquidMaterial liquid = null;

    void Start()
    {
        particleMaterial = particleRenderer.material;
        particleMaterial.color = Color.white;
        particleMaterial.SetColor("_EmissionColor", Color.white);
    }

    void PourControl()
    {
        flow = maxFlow * lever.angle / lever.maxAngle;
        if (cauldron.fillLevel == 0f) flow = 0f;
        particleAnimator.SetFloat("pourSpeed", flow);
        
        if (flow > 0f) cauldron.liquid.FillBump(flow);

        cauldron.ReduceFill(flow * pourMultiplier * Time.deltaTime);
    }

    void StopPour()
    {
        if (flow > 0f)
        {
            flow = 0f;
            particleAnimator.SetFloat("pourSpeed", flow);
        }
    }

    void DumpControl()
    {
        if (particleMaterial.color != cauldron.fillColor)
        {
            liquid = null;
            particleMaterial.color = cauldron.fillColor;
            particleMaterial.SetColor("_EmissionColor", cauldron.fillColor);
            lever.pickup.InteractionText = "Dump Contents";
            lever.pickup.UseText = "Dump Contents";
        }
        PourControl();
    }

    public void SetParticleProperties(LiquidMaterial newLiquid)
    {
        Debug.LogFormat("{0}: setting particle properties: {1}", name, newLiquid.name);
        liquid = newLiquid;
        particleMaterial.color = liquid.color;
        particleMaterial.SetColor("_EmissionColor", liquid.color);
        lever.pickup.InteractionText = liquid.name;
        lever.pickup.UseText = liquid.name;
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

    private void Update()
    {
        if (indicator.IsValid() && cauldron.indicator.IsValid())
        {
            if (cauldron.matchingRecipe != null && liquid != cauldron.matchingRecipe.potion) SetParticleProperties(cauldron.matchingRecipe.potion);
            PourControl();
        }
        else if (indicator.IsNeutral()) DumpControl();
        else StopPour();
        
        if (bottleSnap.GetBottle() != null)
        {
            if (bottleSnap.CheckFill(0f))
            {
                indicator.SetValid();
            }
            else if (!bottleSnap.CheckFill(1f) && bottleSnap.CheckMatch(cauldron.matchingRecipe))
            {
                indicator.SetValid();
            }
            else indicator.SetInvalid();
        }
        else indicator.SetNeutral();
    }

}
