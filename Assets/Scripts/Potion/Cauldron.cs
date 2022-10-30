
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
    [UdonSynced] float fillLevel = 0f;
    [UdonSynced] Color fillColor = Color.white;

    void Start()
    {
        overflowMaterial = overflowParticleRenderer.material;
    }

    public override void OnDeserialization()
    {
        if (liquid != null)
        {
            liquid.fillLevel = fillLevel;
            liquid.SetStaticColor(fillColor);
        }
    }

    private void UpdateFillColor(Color newColor, float fillAmount)
    {
        if (liquid.fillLevel == 0) fillColor = newColor;
        else fillColor = Color.Lerp(fillColor, newColor, fillAmount / fillLevel);
    }

    public void AddLiquid(PourableBottle bottle)
    {
        Debug.Log("Adding Liquid");
        float fillAmount = (bottle.pourSpeed * bottle.pourMultiplier * Time.deltaTime) / maxFill;
        fillLevel += fillAmount;
        liquid.fillLevel = fillLevel;

        UpdateFillColor(bottle.potionColor, fillAmount);

        liquid.SetStaticColor(fillColor);

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
