
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Cauldron : UdonSharpBehaviour
{
    public PotionWobble liquid;
    public Animator liquidColliderAnimator;
    public Animator overflowAnimator;
    public Renderer overflowParticleRenderer;
    public Material overflowMaterial;
    public float overflowFlowRate = 0.02f;
    public float maxFill = 5f;
    public bool isOverflowing = false;

    void Start()
    {
        overflowMaterial = overflowParticleRenderer.material;
    }

    public void AddLiquid(PourableBottle bottle)
    {
        Debug.Log("Adding Liquid");
        float fillAmount = (bottle.pourSpeed * bottle.pourMultiplier * Time.deltaTime) / maxFill;
        liquid.fillLevel += fillAmount;
        if (liquid.fillLevel == 0) liquid.SetStaticColor(bottle.potionColor);
        else liquid.UpdateFillColor(bottle.potionColor, fillAmount);

        if (liquid.fillLevel > 1)
        {
            isOverflowing = true;
        }
        else isOverflowing = false;

        liquidColliderAnimator.SetFloat("FillLevel", liquid.fillLevel);
    }

    private void Update()
    {
        if (isOverflowing)
        {
            overflowMaterial.color = liquid.GetColor();
            overflowAnimator.SetFloat("pourSpeed", 0.4f);
            liquid.fillLevel -= overflowFlowRate * Time.deltaTime;
            if (liquid.fillLevel < 1)
            {
                liquid.fillLevel = 1f;
                overflowAnimator.SetFloat("pourSpeed", 0.0f);
                isOverflowing = false;
            }
        }
        else if (overflowAnimator.GetFloat("pourSpeed") > 0)
        {
            overflowAnimator.SetFloat("pourSpeed", 0.0f);
        }
    }
}
