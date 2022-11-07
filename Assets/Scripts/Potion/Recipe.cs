
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
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

    public bool CheckRecipe(Recipe recipe)
    {
        if (recipe.nReagents != nReagents) return false;

        bool recipeCheck = true;
        if (!(recipeCheck && reagent0 != null && recipe.CheckReagent(reagent0))) recipeCheck = false;
        if (!(recipeCheck && reagent1 != null && recipe.CheckReagent(reagent1))) recipeCheck = false;
        if (!(recipeCheck && reagent2 != null && recipe.CheckReagent(reagent2))) recipeCheck = false;
        if (!(recipeCheck && reagent3 != null && recipe.CheckReagent(reagent3))) recipeCheck = false;
        if (!(recipeCheck && reagent4 != null && recipe.CheckReagent(reagent4))) recipeCheck = false;

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
}
