
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ThrowablePotion : BottleCollision
{
    public ShatterEffect shatterEffect;

    public override void Shatter()
    {
        base.Shatter();
        shatterEffect.OnShatter();
    }
}
