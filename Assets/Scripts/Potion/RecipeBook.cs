
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using System.Runtime.CompilerServices;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RecipeBook : UdonSharpBehaviour
{
    public Recipes recipeList;
    [UdonSynced] public int currentRecipeIndex = 0;
    public RecipePanel[] panels;
    public Text potionText;
    public Animator animator;

    public override void OnDeserialization() {
        UpdateRecipe();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal) {
            SendCustomEventDelayedFrames("UpdateRecipe", 100);
        }
        RequestSerialization();
    }

    public void UpdateAnimator() {
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("Armature|Jangle")) {
            animator.SetLayerWeight(1, Mathf.Clamp(animator.GetLayerWeight(1) + 0.2f, 0f, 1f));
        }
        else {
            animator.SetLayerWeight(1, 0.2f);
        }
        animator.SetBool("Jangle", true);
        SendCustomEventDelayedFrames(nameof(Unjangle), 5);
    }

    public void Unjangle() {
        animator.SetBool("Jangle", false);
    }

    public void NextRecipe() {
        UpdateAnimator();
        bool firstRun = true;
        while (firstRun || !recipeList.recipes[currentRecipeIndex].unlocked) {
            currentRecipeIndex += 1;
            if (currentRecipeIndex >= recipeList.recipes.Length) {
                currentRecipeIndex = 0;
            }
            UpdateRecipe();
            firstRun = false;
        }
        RequestSerialization();
    }

    public void PreviousRecipe() {
        UpdateAnimator();
        bool firstRun = true;
        while (firstRun || !recipeList.recipes[currentRecipeIndex].unlocked) {
            currentRecipeIndex -= 1;
            if (currentRecipeIndex < 0) {
                currentRecipeIndex = recipeList.recipes.Length - 1;
            }
            UpdateRecipe();
            firstRun = false;
        }
        RequestSerialization();
    }

    public void UpdateRecipe() {
        ClearRecipe();

        Recipe currentRecipe = recipeList.recipes[currentRecipeIndex];
        if (currentRecipe == null) {
            Debug.LogFormat("{0}: Tried to update with null recipe [{1}]", name, currentRecipeIndex);
            return;
        }
        if (currentRecipe.potion == null) {
            Debug.LogFormat("{0}: Tried to update with null potion: {1} [{2}]", name, currentRecipe.name, currentRecipeIndex);
            return;
        }
        Debug.LogFormat("{0}: Updating with recipe: {1} [{2}]", name, currentRecipe.potion.name, currentRecipeIndex);
        if (!currentRecipe.unlocked) {
            // string[] potionStrs = currentRecipe.potion.name.Split(" ");
            // if (potionStrs.Length > 1) {
            //     potionText.text = potionStrs[0];
            //     for (int i = 1; i < potionStrs.Length; i++) {
            //         potionText.text += " ";
            //         for (int j = 0; j < potionStrs[i].Length; j++) {
            //             potionText.text += "?";
            //         }
            //     }

            // }
            // else {
            //     potionText.text = "";
            //     for (int j = 0; j < potionStrs[0].Length; j++) {
            //         potionText.text += "?";
            //     }
            // }
            return;
        }
        potionText.text = currentRecipe.potion.name;
        UpdateRow(currentRecipe.partsReagent0, currentRecipe.reagent0, 0);
        UpdateRow(currentRecipe.partsReagent1, currentRecipe.reagent1, 5);
        UpdateRow(currentRecipe.partsReagent2, currentRecipe.reagent2, 10);
        UpdateRow(currentRecipe.partsReagent3, currentRecipe.reagent3, 15);
        UpdateRow(currentRecipe.partsReagent4, currentRecipe.reagent4, 20);
    }

    public void UpdateRow(int nPartsReagent, LiquidMaterial reagent, int rowOffset) {
        for (int i=0; i < nPartsReagent; i++) {
            int targetIndex = i < 5 ? i + rowOffset : i + rowOffset - 5;
            bool wrap = i >= 5;
            Debug.LogFormat("{0}: UpdateRow -- targetIndex: {1}  wrap: {2}", name, targetIndex, wrap);
            if (targetIndex >= panels.Length) {
                Debug.LogFormat("{0}: Tried to access out-of-bounds array index {1}", name, targetIndex);
            }
            else {
                panels[targetIndex].SetPanel(reagent, wrap);
            }
        }
    }

    public void ClearRecipe() {
        foreach (RecipePanel panel in panels) {
            panel.ResetPanel();
        }
    }

    public void Start() {
        SendCustomEventDelayedFrames("UpdateRecipe", 5);
    }
}
