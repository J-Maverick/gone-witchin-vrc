
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LiveRecipe : UdonSharpBehaviour
{
    public Cauldron cauldron;
    public RecipePanel[] panels;

    public override void OnDeserialization() {
        UpdateRecipe();
    }

    public void UpdateRecipe() {
        ClearRecipe();

        UpdateRow(cauldron.fillRecipe.partsReagent0, cauldron.fillRecipe.reagent0, 0);
        UpdateRow(cauldron.fillRecipe.partsReagent1, cauldron.fillRecipe.reagent1, 5);
        UpdateRow(cauldron.fillRecipe.partsReagent2, cauldron.fillRecipe.reagent2, 10);
        UpdateRow(cauldron.fillRecipe.partsReagent3, cauldron.fillRecipe.reagent3, 15);
        UpdateRow(cauldron.fillRecipe.partsReagent4, cauldron.fillRecipe.reagent4, 20);
    }

    public void UpdateRow(int nPartsReagent, LiquidMaterial reagent, int rowOffset) {
        for (int i=0; i < nPartsReagent; i++) {
            int targetIndex = i < 5 ? i + rowOffset : i + rowOffset - 5;
            bool wrap = i >= 5;
            Debug.LogFormat("{0}: UpdateRow -- targetIndex: {1}  wrap: {2}", name, targetIndex, wrap);
            if (targetIndex >= panels.Length) {
                Debug.LogFormat("{0}: Tried to access out-of-bounds array index {1}", name, targetIndex);
            }
            else if (i >= 10) {
                break;
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
