
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
    public GemIndicator indicator;
    public float overflowFlowRate = 0.02f;
    public float maxFill = 5f;
    public bool isOverflowing = false;
    [UdonSynced] float fillLevel = 0f;
    [UdonSynced] Color fillColor = Color.white;
    public CauldronRecipe fillRecipe;
    public Recipes recipes;
    public Recipe matchingRecipe = null;
    public bool ratioMatched = false;

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
        if (fillRecipe.isDS)
        {
            fillRecipe.NormalizeReagents();
            matchingRecipe = recipes.GetMatchingRecipe(fillRecipe);
            if (matchingRecipe != null)
            {
                ratioMatched = matchingRecipe.CheckRecipeRatio(fillRecipe);
                Debug.LogFormat("{0}: Current Recipe: {1} -- Ratio Matched: {2}", name, matchingRecipe.name, ratioMatched);
            }
            else Debug.LogFormat("{0}: Current Recipe: null -- Ratio Matched: {1}", name, ratioMatched);

            fillRecipe.LogReagents();
        }
    }

    private void UpdateFillColor(Color newColor, float fillAmount)
    {
        if (liquid.fillLevel == 0) fillColor = newColor;
        else fillColor = Color.Lerp(fillColor, newColor, fillAmount / fillLevel);
    }

    public void AddLiquid(ReagentBottle bottle)
    {
        Networking.SetOwner(Networking.GetOwner(bottle.gameObject), gameObject);
        Debug.Log("Adding Liquid");
        float fillAmount = (bottle.pourSpeed * bottle.pourMultiplier * Time.deltaTime) / maxFill;
        fillLevel += fillAmount;
        liquid.fillLevel = fillLevel;

        fillRecipe.AddReagent(bottle.reagent, fillAmount);

        UpdateFillColor(bottle.potionColor, fillAmount);

        liquid.FillBump(fillAmount * 500f);
        liquid.SetStaticColor(fillColor);

        if (liquid.fillLevel > 1)
        {
            isOverflowing = true;
        }
        else isOverflowing = false;

        liquidColliderAnimator.SetFloat("FillLevel", liquid.fillLevel);

        matchingRecipe = recipes.GetMatchingRecipe(fillRecipe);
        if (matchingRecipe != null)
        {
            ratioMatched = matchingRecipe.CheckRecipeRatio(fillRecipe);
            Debug.LogFormat("{0}: Current Recipe: {1} -- Ratio Matched: {2}", name, matchingRecipe.name, ratioMatched);
        }
        else
        {
            ratioMatched = false;
            Debug.LogFormat("{0}: Current Recipe: null -- Ratio Matched: {1}", name, ratioMatched);
        }

        fillRecipe.LogReagents();
        RequestSerialization();
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

        if (ratioMatched != indicator.IsValid())
        {
            if (ratioMatched) indicator.SetValid();
            else indicator.SetInvalid();
        }
    }
}
