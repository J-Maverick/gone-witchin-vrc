
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CleansingPotion : DrinkEffect
{
    public PlayerStatBooster playerStatBooster = null;

    public override void OnDrink()
    {
        if (playerStatBooster != null) playerStatBooster.ResetAllModifiers();
    }
}
