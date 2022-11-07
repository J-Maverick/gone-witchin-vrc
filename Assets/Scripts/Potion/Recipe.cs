
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Recipe : UdonSharpBehaviour
{
    public Potion potion;
    public int nReagents;
    public Reagent reagent0 = null;
    public int partsReagent0 = 0;
    public Reagent reagent1 = null;
    public int partsReagent1 = 0;
    public Reagent reagent2 = null;
    public int partsReagent2 = 0;
    public Reagent reagent3 = null;
    public int partsReagent3 = 0;
    public Reagent reagent4 = null;
    public int partsReagent4 = 0;

    public ReagentList reagentList;

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

    public bool CheckReagent(Reagent reagent)
    {
        if (reagent0 == reagent) return true;
        if (reagent1 == reagent) return true;
        if (reagent2 == reagent) return true;
        if (reagent3 == reagent) return true;
        if (reagent4 == reagent) return true;

        return false;
    }

    public bool CheckReagentRatio(Reagent reagent, int nParts)
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
}
