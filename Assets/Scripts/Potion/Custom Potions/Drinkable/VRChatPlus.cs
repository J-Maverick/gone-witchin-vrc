
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRChatPlus : DrinkEffect
{
    public float effectDuration = 300f;
    public PlayerStatBooster playerStatBooster = null;

    public override void OnDrink()
    {
        if (playerStatBooster != null) playerStatBooster.ActivateDoubleJump(effectDuration);
    }
}
