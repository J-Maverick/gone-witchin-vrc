
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReagentTank : UdonSharpBehaviour
{
    public Reagent reagent;
    public PotionWobble shaderControl;
    public Renderer particleRenderer;
    public Material particleMaterial;
    public Animator particleAnimator;

    public Lever lever;
    public ReagentBottle targetBottle;

    public float maxFlow = 0.6f;
    public float flow = 0f;
    public float fillLevel = 1f;
    public float pourMultiplier = 0.1f;

    void Start()
    {
        shaderControl.SetColor(reagent.color);
        particleMaterial = particleRenderer.material;
        particleMaterial.color = reagent.color;
        fillLevel = Random.Range(0.05f, 1f);
        shaderControl.fillLevel = fillLevel;
        lever.pickup.InteractionText = reagent.name;
    }

    void PourControl()
    {
        flow = maxFlow * lever.angle / lever.maxAngle;
        if (fillLevel == 0f) flow = 0f;
        particleAnimator.SetFloat("pourSpeed", flow);

        if (flow > 0f) shaderControl.FillBump(flow);

        fillLevel -= flow * pourMultiplier * Time.deltaTime;
        if (fillLevel < 0f) fillLevel = 0f;
        if (shaderControl != null) shaderControl.fillLevel = fillLevel;
    }

    private void Update()
    {
        PourControl();
    }

}
