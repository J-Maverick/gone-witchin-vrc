
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
    public MeshRenderer pipeSpurtRenderer;
    public Material pipeSpurtMaterial;
    private float overflowFlowRate = 0.05f;
    private float overFlowExponent = 4f;
    public float maxFill = 5f;
    public bool isOverflowing = false;
    [UdonSynced] public float fillLevel = 0f;
    [UdonSynced] public Color fillColor = Color.white;
    public CauldronRecipe fillRecipe;
    public Recipes recipes;
    public Recipe matchingRecipe = null;
    public bool ratioMatched = false;
    public bool impossibleRecipe = false;
    public LiveRecipe liveRecipe;
    public AudioSource cauldronAudio;

    void Start()
    {
        overflowMaterial = overflowParticleRenderer.material;
        pipeSpurtMaterial = pipeSpurtRenderer.material;
    }

    public override void OnDeserialization()
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
        if (liquid != null)
        {
            liquid.fillLevel = fillLevel;
            if (matchingRecipe != null)
            {
                fillRecipeLerp(matchingRecipe.RecipeNearRatio(fillRecipe));
            }
            else {
                liquid.SetStaticColor(fillColor);
            }
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
        if (fillLevel > 1)
        {
            isOverflowing = true;
            liquid.fillLevel = Mathf.Pow(fillLevel, overFlowExponent);
        }
        else 
        {
            isOverflowing = false;
            overflowAnimator.SetFloat("pourSpeed", 0.0f);
            liquid.fillLevel = fillLevel;
        }

        fillRecipe.AddReagent(bottle.liquid, fillAmount);
        liveRecipe.UpdateRecipe();

        UpdateFillColor(bottle.potionColor, fillAmount);

        liquid.FillBump(fillAmount * 500f);
        liquid.SetStaticColor(fillColor);



        liquidColliderAnimator.SetFloat("FillLevel", fillLevel);

        if (!impossibleRecipe) impossibleRecipe = fillRecipe.nReagents == -1 || recipes.RecipeIsImpossible(fillRecipe);

        if (!impossibleRecipe)
        {
            matchingRecipe = recipes.GetMatchingRecipe(fillRecipe);

            if (matchingRecipe != null)
            {
                ratioMatched = matchingRecipe.CheckRecipeRatio(fillRecipe);
                fillRecipeLerp(matchingRecipe.RecipeNearRatio(fillRecipe));

                if (ratioMatched && !matchingRecipe.unlocked) {
                    matchingRecipe.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(matchingRecipe.Unlock));
                }
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

    public void fillRecipeLerp(float ratio) {
        Debug.LogFormat("{0}: Lerping color by ratio: {1}", name, ratio);
        liquid.SetStaticColor(Color.Lerp(fillColor, matchingRecipe.potion.color, ratio));
        pipeSpurtMaterial.SetColor("_DepthColour", matchingRecipe.potion.color);
    }

    public void UpdateFill()
    {
        if (fillLevel > 1) {
            liquid.fillLevel = Mathf.Pow(fillLevel, overFlowExponent);
        }
        else {
            liquid.fillLevel = fillLevel;
        }
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
                liquid.fillLevel = fillLevel;
            }
            else {
                liquid.fillLevel = Mathf.Pow(fillLevel, overFlowExponent);
            }
        }

        if (fillLevel > 0f)
        {
            if (ratioMatched) {
                indicator.SetValid();
                pipeSpurtRenderer.enabled = true;
            }
            else if (impossibleRecipe) {
                indicator.SetInvalid();
                pipeSpurtRenderer.enabled = false;
            }
            else {
                indicator.SetNeutral();
                pipeSpurtRenderer.enabled = false;
            }
            cauldronAudio.enabled = true;
            cauldronAudio.pitch = Mathf.Lerp(0.25f, 0.5f, fillLevel / maxFill);
        }
        else {
            indicator.SetNeutral();
            pipeSpurtRenderer.enabled = false;
            cauldronAudio.enabled = false;
        }

        if (fillLevel == 0f && fillRecipe.nReagents > 0)
        {
            fillRecipe.ResetReagents();
            liveRecipe.UpdateRecipe();
            impossibleRecipe = false;
        }
    }
}
