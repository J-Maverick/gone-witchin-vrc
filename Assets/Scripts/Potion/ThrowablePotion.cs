
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ThrowablePotion : BottleCollision
{
    public ShatterEffect shatterEffect;
    public int restrictedLayer = -1;
    private bool validCollision = true;

    public override void OnCollisionEnter(Collision collision) {
        if (restrictedLayer != -1) {
            validCollision = collision.gameObject.layer == restrictedLayer;
        }
        base.OnCollisionEnter(collision);
    }

    public override void Shatter()
    {
        base.Shatter();
        if (shatterEffect != null && validCollision) {
            shatterEffect.OnShatter();
        }
    }
}
