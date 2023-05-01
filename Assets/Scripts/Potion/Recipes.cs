
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Recipes : UdonSharpBehaviour
{
    public Recipe[] recipes;

    public Recipe GetMatchingRecipe(Recipe recipeToMatch)
    {
        Debug.LogFormat("{0}: Checking recipes...", name);
        foreach (Recipe recipe in recipes)
        {
            if (recipe.CheckRecipe(recipeToMatch))
            {
                Debug.LogFormat("{0}: Found match -- {1}", name, recipe.name);
                return recipe;
            }
        }
        Debug.LogFormat("{0}: No Matches found.", name);
        return null;
    }

    public bool RecipeIsImpossible(Recipe recipeToMatch)
    {
        foreach (Recipe recipe in recipes)
        {
            if (!recipe.CheckImpossible(recipeToMatch))
            {
                return false;
            }
        }
        return true;
    }
}
