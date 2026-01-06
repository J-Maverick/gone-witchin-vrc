
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class LiquidList : UdonSharpBehaviour
{
    public LiquidMaterial[] liquids;

    public LiquidMaterial GetLiquidByID(int ID)
    {
        foreach (LiquidMaterial liquid in liquids)
        {
            if (liquid.ID == ID) return liquid;
        }
        return null;
    }

    [ContextMenu("Guarantee Unique IDs")]
    public void GuaranteeUniqueIDs()
    {
        for (int i = 0; i < liquids.Length; i++)
        {
            // Detect if liquid is a reagent
            liquids[i].ID = 1000 + i;
        }
    }

}
