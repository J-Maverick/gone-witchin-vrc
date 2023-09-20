
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Flashbang : ShatterEffect
{
    public Animator flashAnimator;
    public BottleCollision bottleCollision;
    public SphereCollider potionCollider;

    public float collisionEnabledTime = 0.5f;
    public float collisionEnabledTimer = 0f;

    bool effectActive = false;

    private void Start()
    {
        potionCollider.enabled = false;
    }

    public override void OnShatter()
    {
        effectActive = true;
        potionCollider.enabled = true;
        SendCustomEventDelayedSeconds("DisableEffect", collisionEnabledTime);
    }

    public void DisableEffect() {
        potionCollider.enabled = false;
        effectActive = false;
        flashAnimator.SetBool("Flash", false);
    }

    public void Flash(VRCPlayerApi player)
    {
        if (effectActive)
        {
            if (player.isLocal)
            {
                flashAnimator.SetBool("Flash", true);
            }
        }
    }
}
