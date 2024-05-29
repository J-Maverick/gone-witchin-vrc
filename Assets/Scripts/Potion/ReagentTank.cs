
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ReagentTank : UdonSharpBehaviour
{
    public LiquidMaterial reagent;
    public PotionWobble shaderControl;
    public Renderer fishRenderer;
    public Material fishMaterial;
    public Renderer particleRenderer;
    public Material particleMaterial;
    public Animator particleAnimator;
    public MeshRenderer labelRenderer;
    public Material labelMaterial;

    public Lever lever;

    public float maxFlow = 0.6f;
    public float minFlow = 0.1f;
    public float flowReductionFill = 0.1f;
    public float currentMaxFlow = 1f;
    public float flow = 0f;
    [UdonSynced] public float fillLevel = 0f;
    public float pourMultiplier = 0.1f;

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

        labelMaterial = labelRenderer.material;
        labelMaterial.SetTextureOffset("_MainTex", new Vector2(reagent.UVOffsetX, reagent.UVOffsetY));
        labelMaterial.SetColor("_EmissionColor", reagent.color);

        fishMaterial = fishRenderer.materials[0];
        fishMaterial.SetColor("_Color", reagent.color);


        shaderControl.fillLevel = fillLevel;
    }

    void PourControl()
    {
        if (!lever.isSleeping)
        {
            currentMaxFlow = fillLevel > flowReductionFill ? maxFlow : minFlow + ((fillLevel / flowReductionFill) * (maxFlow - minFlow));
            flow = currentMaxFlow * lever.angle / lever.maxAngle;
            if (fillLevel == 0f) flow = 0f;
            particleAnimator.SetFloat("pourSpeed", flow);

            if (flow > 0f) shaderControl.FillBump(5f * flow);

            fillLevel -= flow * pourMultiplier * Time.deltaTime;
            if (fillLevel < 0f) fillLevel = 0f;
            if (shaderControl != null) UpdateFill();

            if (Time.frameCount % 10 == 0) Sync();
        }
    }

    private void Update()
    {
        PourControl();
    }

    public void JoinSync() {
        if (joinSyncCounter < nJoinSyncs) {
            SendCustomEventDelayedSeconds("JoinSync", intervalTime);
            Sync();
            joinSyncCounter++;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        joinSyncCounter = 0;
        JoinSync();
    }

    void UpdateFill()
    {
        shaderControl.fillLevel = fillLevel;
    }

    public void Sync()
    {
        UpdateFill();
        RequestSerialization();
    }

    public override void OnDeserialization() {
        UpdateFill();
    }
}
