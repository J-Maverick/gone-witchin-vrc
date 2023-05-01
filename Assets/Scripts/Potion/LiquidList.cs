
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

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
}
