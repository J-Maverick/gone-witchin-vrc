
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Recipe : UdonSharpBehaviour
{
    public LiquidMaterial potion;
    [UdonSynced] public bool unlocked = true;
    public int nReagents;
    public LiquidMaterial reagent0 = null;
    public int partsReagent0 = 0;
    public LiquidMaterial reagent1 = null;
    public int partsReagent1 = 0;
    public LiquidMaterial reagent2 = null;
    public int partsReagent2 = 0;
    public LiquidMaterial reagent3 = null;
    public int partsReagent3 = 0;
    public LiquidMaterial reagent4 = null;
    public int partsReagent4 = 0;

    public ReagentList reagentList;

    public AudioSource fanfareAudio;
    public RecipeBook recipeBook;

    private void Start()
    {
        LiquidMaterial potionComponent = GetComponent<LiquidMaterial>();
        if (potionComponent != null)
        {
            potion = potionComponent;
        }
    }

    public void Unlock() {
        if (!unlocked) {
            unlocked = true;
            PlayFanfare();

            if (this == recipeBook.recipeList.recipes[recipeBook.currentRecipeIndex]) {
                recipeBook.UpdateRecipe();
                recipeBook.RequestSerialization();
            }
        }
        RequestSerialization();
    }

    public void PlayFanfare() {
        fanfareAudio.Play();
    }

    public bool CheckRecipe(Recipe recipe)
    {
        if (recipe.nReagents != nReagents) return false;
        Debug.LogFormat("{0}: nReagents matched, checking recipe...", name);
        bool recipeCheck = true;
        if (recipeCheck && reagent0 != null && !recipe.CheckReagent(reagent0)) recipeCheck = false;
        if (recipeCheck && reagent1 != null && !recipe.CheckReagent(reagent1)) recipeCheck = false;
        if (recipeCheck && reagent2 != null && !recipe.CheckReagent(reagent2)) recipeCheck = false;
        if (recipeCheck && reagent3 != null && !recipe.CheckReagent(reagent3)) recipeCheck = false;
        if (recipeCheck && reagent4 != null && !recipe.CheckReagent(reagent4)) recipeCheck = false;

        return recipeCheck;
    }

    public bool CheckImpossible(Recipe recipe)
    {
        bool isImpossible = false;
        if (recipe.nReagents > nReagents) isImpossible = true;
        else
        {
            if (!isImpossible && recipe.reagent0 != null && !CheckReagent(recipe.reagent0)) isImpossible = true;
            if (!isImpossible && recipe.reagent1 != null && !CheckReagent(recipe.reagent1)) isImpossible = true;
            if (!isImpossible && recipe.reagent2 != null && !CheckReagent(recipe.reagent2)) isImpossible = true;
            if (!isImpossible && recipe.reagent3 != null && !CheckReagent(recipe.reagent3)) isImpossible = true;
            if (!isImpossible && recipe.reagent4 != null && !CheckReagent(recipe.reagent4)) isImpossible = true;
        }
        return isImpossible;
    }

    public bool CheckReagent(LiquidMaterial reagent)
    {
        if (reagent0 == reagent) return true;
        if (reagent1 == reagent) return true;
        if (reagent2 == reagent) return true;
        if (reagent3 == reagent) return true;
        if (reagent4 == reagent) return true;

        return false;
    }
float GetReagentPartsAsFloat(LiquidMaterial reagent)
    {
        if (reagent0 == reagent) return partsReagent0;
        if (reagent1 == reagent) return partsReagent1;
        if (reagent2 == reagent) return partsReagent2;
        if (reagent3 == reagent) return partsReagent3;
        if (reagent4 == reagent) return partsReagent4;

        return 0f;
    }

    public bool CheckReagentRatio(LiquidMaterial reagent, int nParts)
    {
        if (reagent0 == reagent && partsReagent0 == nParts) return true;
        if (reagent1 == reagent && partsReagent1 == nParts) return true;
        if (reagent2 == reagent && partsReagent2 == nParts) return true;
        if (reagent3 == reagent && partsReagent3 == nParts) return true;
        if (reagent4 == reagent && partsReagent4 == nParts) return true;

        return false;
    }

    public void ClearRecipe()
    {
        nReagents = 0;
        reagent0 = null;
        partsReagent0 = 0;
        reagent1 = null;
        partsReagent1 = 0;
        reagent2 = null;
        partsReagent2 = 0;
        reagent3 = null;
        partsReagent3 = 0;
        reagent4 = null;
        partsReagent4 = 0;
    }

    public bool CheckRecipeRatio(Recipe recipe)
    {
        bool recipeRatioCheck = true;

        if (CheckRecipe(recipe))
        {
            if (recipe.reagent0 != null && !CheckReagentRatio(recipe.reagent0, recipe.partsReagent0)) recipeRatioCheck = false;
            if (recipe.reagent1 != null && !CheckReagentRatio(recipe.reagent1, recipe.partsReagent1)) recipeRatioCheck = false;
            if (recipe.reagent2 != null && !CheckReagentRatio(recipe.reagent2, recipe.partsReagent2)) recipeRatioCheck = false;
            if (recipe.reagent3 != null && !CheckReagentRatio(recipe.reagent3, recipe.partsReagent3)) recipeRatioCheck = false;
            if (recipe.reagent4 != null && !CheckReagentRatio(recipe.reagent4, recipe.partsReagent4)) recipeRatioCheck = false;
        }
        else recipeRatioCheck = false;

        return recipeRatioCheck;
    }

    public float RecipeNearRatio(CauldronRecipe recipe) {
        float ratio = 0f;
        float minReagentFill = recipe.MinReagentFill();
        if (recipe.reagent0 != null) {
            float reagentPart = GetReagentPartsAsFloat(recipe.reagent0);
            ratio += (1 - (Mathf.Abs((recipe.fillReagent0 / minReagentFill) - reagentPart) / reagentPart)) / nReagents;
        };
        if (recipe.reagent1 != null) {
            float reagentPart = GetReagentPartsAsFloat(recipe.reagent1);
            ratio += (1 - (Mathf.Abs((recipe.fillReagent1 / minReagentFill) - reagentPart) / reagentPart)) / nReagents;
        };
        if (recipe.reagent2 != null) {
            float reagentPart = GetReagentPartsAsFloat(recipe.reagent2);
            ratio += (1 - (Mathf.Abs((recipe.fillReagent2 / minReagentFill) - reagentPart) / reagentPart)) / nReagents;
        };
        if (recipe.reagent3 != null) {
            float reagentPart = GetReagentPartsAsFloat(recipe.reagent3);
            ratio += (1 - (Mathf.Abs((recipe.fillReagent3 / minReagentFill) - reagentPart) / reagentPart)) / nReagents;
        };
        if (recipe.reagent4 != null) {
            float reagentPart = GetReagentPartsAsFloat(recipe.reagent4);
            ratio += (1 - (Mathf.Abs((recipe.fillReagent4 / minReagentFill) - reagentPart) / reagentPart)) / nReagents;
        };

        float minRatio = 0.7f;
        if (ratio < minRatio) {
            ratio = 0f;
        }
        else {
            ratio = (ratio - minRatio) / (1f - minRatio);
        }

        return ratio;
    }
}
