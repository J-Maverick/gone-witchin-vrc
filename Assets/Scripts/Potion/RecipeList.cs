using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RecipeList : MonoBehaviour
{
    public Recipe[] recipes;
    public string recipeList;
    public Potion[] potions;

    private void Awake()
    {
        //CheckMatches();
        //UpdateString();
        //RandomizeColors();
        SetIds();
    }


    private void RandomizeColors()
    {
        foreach (Potion potion in potions)
        {
            potion.color = Random.ColorHSV();
        }
    }

    private void SetIds()
    {
        int ID = 1000;
        foreach (Potion potion in potions)
        {
            potion.ID = ID;
            ID++;
        }
    }

    private void CheckMatches()
    {
        Debug.LogFormat("{0}: Checking for matching recipes...", name);
        bool matchFound = false;
        foreach (Recipe recipe in recipes)
        {
            foreach (Recipe otherRecipe in recipes)
            {
                if (recipe != otherRecipe)
                {
                    if (recipe.CheckRecipe(otherRecipe))
                    {
                        Debug.LogFormat("{0}: {1} matches {2}", name, recipe.name, otherRecipe.name);
                        matchFound = true;
                    }
                    else Debug.LogFormat("{0}: {1} does not match {2}", name, recipe.name, otherRecipe.name);
                }
            }
        }
        if (matchFound)
            Debug.LogFormat("{0}: Match Found! Fix it!", name);
        else Debug.LogFormat("{0}: No matches found.", name);
    }

    private void UpdateString()
    {
        foreach (Recipe recipe in recipes)
        {
            string recipeString = string.Format("{0}: ", recipe.name);
            if (recipe.reagent0 != null) recipeString += string.Format("{0} {1}", recipe.partsReagent0, recipe.reagent0.name);
            if (recipe.reagent1 != null) recipeString += string.Format(" + {0} {1}", recipe.partsReagent1, recipe.reagent1.name);
            if (recipe.reagent2 != null) recipeString += string.Format(" + {0} {1}", recipe.partsReagent2, recipe.reagent2.name);
            if (recipe.reagent3 != null) recipeString += string.Format(" + {0} {1}", recipe.partsReagent3, recipe.reagent3.name);
            if (recipe.reagent4 != null) recipeString += string.Format(" + {0} {1}", recipe.partsReagent4, recipe.reagent4.name);
            recipeString += "\n";
            recipeList += recipeString;
        }
    }
}
