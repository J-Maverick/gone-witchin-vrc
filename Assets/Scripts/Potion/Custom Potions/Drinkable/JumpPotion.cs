
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class JumpPotion : DrinkEffect
{
    public float effectStrength = 2f;
    public float effectDuration = 120f;
    public PlayerStatBooster statBooster = null;
    public BoostStackingMode boostStackingMode = BoostStackingMode.Linear;

    public override void OnDrink()
    {
        if (statBooster != null)
        {
            statBooster.BoostJumpImpulse(effectStrength, effectDuration, boostStackingMode);
        }
    }
}
