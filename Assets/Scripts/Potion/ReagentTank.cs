
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ReagentTank : UdonSharpBehaviour
{
    public LiquidMaterial reagent;
    public PotionWobble shaderControl;
    public Renderer particleRenderer;
    public Material particleMaterial;
    public Animator particleAnimator;

    public Lever lever;

    public float maxFlow = 0.6f;
    public float flow = 0f;
    [UdonSynced] public float fillLevel = 0f;
    public float pourMultiplier = 0.1f;

    private float nextTime = 0f;
    public float intervalTime = 3f;
    private int nJoinSyncs = 10;
    public int joinSyncCounter = 0;

    void Start()
    {
        //if (Networking.LocalPlayer.isMaster) fillLevel = Random.Range(0.05f, 1f);
        shaderControl.SetColor(reagent.color);
        particleMaterial = particleRenderer.material;
        particleMaterial.color = reagent.color;
        particleMaterial.SetColor("_EmissionColor", reagent.color);
        lever.pickup.InteractionText = reagent.name;
    }

    void PourControl()
    {
        if (!lever.isSleeping)
        {
            flow = maxFlow * lever.angle / lever.maxAngle;
            if (fillLevel == 0f) flow = 0f;
            particleAnimator.SetFloat("pourSpeed", flow);

            if (flow > 0f) shaderControl.FillBump(flow);

            fillLevel -= flow * pourMultiplier * Time.deltaTime;
            if (fillLevel < 0f) fillLevel = 0f;
            if (shaderControl != null) UpdateFill();
        }
    }

    private void Update()
    {
        PourControl();
        if (joinSyncCounter < nJoinSyncs && Time.realtimeSinceStartup > nextTime)
        {
            RequestSerialization();
            joinSyncCounter++;
            nextTime = Time.realtimeSinceStartup + intervalTime;
        }
    }

    void UpdateFill()
    {
        shaderControl.fillLevel = fillLevel;
    }

    public void Sync()
    {
        RequestSerialization();
    }
}
