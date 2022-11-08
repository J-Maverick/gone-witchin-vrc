
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ThrowablePotion : BottleCollision
{
    public override void Shatter()
    {
        base.Shatter();
        ShatterEffect();
    }

    public virtual void ShatterEffect()
    {

    }
}
