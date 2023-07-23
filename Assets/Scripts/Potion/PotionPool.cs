
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class PotionPool : BottleSpawner
{
    public LiquidMaterial liquid;

    public bool IsMatch(int ID) {
        return liquid.ID == ID;
    }

}
