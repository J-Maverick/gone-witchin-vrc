
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ShillingChalice : ShatterEffect
{
    public SphereCollider potionCollider;

    public float collisionEnabledTime = 2f;

    private void Start()
    {
        potionCollider.enabled = false;
    }

    public override void OnShatter()
    {
        potionCollider.enabled = true;
        SendCustomEventDelayedSeconds("DisableEffect", collisionEnabledTime);
    }

    private void DisableEffect() {
        potionCollider.enabled = false;
    }

}

