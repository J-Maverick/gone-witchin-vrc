
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PotionOfVanity : ShatterEffect
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

    public void DisableEffect() {
        potionCollider.enabled = false;
    }
}

