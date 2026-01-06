
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ThrowablePotion : BottleCollision
{
    public ShatterEffect shatterEffect;
    public int restrictedLayer = -1;
    public int instantShatterLayer = -1;
    private bool validCollision = true;
    public bool sendBypassCollisionEffect = false;

    public override void OnCollisionEnter(Collision collision) {
        if (instantShatterLayer != -1) {
            if (collision.gameObject.layer == instantShatterLayer) {
                validCollision = true;
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Shatter));
                return;
            }
        }
        if (restrictedLayer != -1) {
            validCollision = collision.gameObject.layer == restrictedLayer;
        }
        base.OnCollisionEnter(collision);
    }

    public void OnTriggerEnter(Collider collision)
    {        
        if (instantShatterLayer != -1) {
            if (collision.gameObject.layer == instantShatterLayer) {
                validCollision = true;
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Shatter));
                return;
            }
        }
    }


    public override void Shatter()
    {
        base.Shatter();
        if (shatterEffect != null && validCollision) {
            shatterEffect.OnShatter();
        }
        if (shatterEffect != null && sendBypassCollisionEffect)
        {
            shatterEffect.BypassShatter();
        }
    }
}
