
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
    private float overflowFlowRate = 0.05f;
    public float maxFill = 5f;
    public bool isOverflowing = false;
    [UdonSynced] public float fillLevel = 0f;
    [UdonSynced] public Color fillColor = Color.white;
    public CauldronRecipe fillRecipe;
    public Recipes recipes;
    public Recipe matchingRecipe = null;
    public bool ratioMatched = false;
    public bool impossibleRecipe = false;

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

        fillRecipe.AddReagent(bottle.liquid, fillAmount);

        UpdateFillColor(bottle.potionColor, fillAmount);

        liquid.FillBump(fillAmount * 500f);
        liquid.SetStaticColor(fillColor);

        if (liquid.fillLevel > 1)
        {
            isOverflowing = true;
        }
        else 
        {
            isOverflowing = false;
            overflowAnimator.SetFloat("pourSpeed", 0.0f);
        }

        liquidColliderAnimator.SetFloat("FillLevel", liquid.fillLevel);

        if (!impossibleRecipe) impossibleRecipe = fillRecipe.nReagents == -1 || recipes.RecipeIsImpossible(fillRecipe);

        if (!impossibleRecipe)
        {
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
        }
        else
        {
            matchingRecipe = null;
            ratioMatched = false;
        }

        fillRecipe.LogReagents();
        RequestSerialization();
    }

    public void UpdateFill()
    {
        liquid.fillLevel = fillLevel;
    }

    public void ReduceFill(float reduceAmount)
    {
        fillRecipe.ReduceFill(fillLevel, reduceAmount);
        if (fillLevel > 0f)
        {
            fillLevel -= reduceAmount;
            if (fillLevel <= 0f)
            {
                fillLevel = 0f;
                ratioMatched = false;
            }
        }
        if (liquid != null) UpdateFill();
    }

    private void Update()
    {
        if (isOverflowing)
        {
            overflowMaterial.color = liquid.GetColor();
            overflowMaterial.SetColor("_EmissionColor", liquid.GetColor());
            overflowAnimator.SetFloat("pourSpeed", 0.4f);
            fillLevel -= overflowFlowRate * Time.deltaTime;
            
            if (fillLevel < 1)
            {
                fillLevel = 1f;
                overflowAnimator.SetFloat("pourSpeed", 0.0f);
                isOverflowing = false;
            }
            liquid.fillLevel = fillLevel;
        }

        if (fillLevel > 0f)
        {
            if (ratioMatched) indicator.SetValid();
            else if (impossibleRecipe) indicator.SetInvalid();
            else indicator.SetNeutral();
        }
        else indicator.SetNeutral();

        if (fillLevel == 0f && fillRecipe.nReagents > 0)
        {
            fillRecipe.ResetReagents();
            impossibleRecipe = false;
        }
    }
}
