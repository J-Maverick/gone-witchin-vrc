
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PotionOfVanity : ShatterEffect
{
    public SphereCollider potionCollider;

    public float collisionEnabledTime = 2f;
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
        collisionEnabledTimer = collisionEnabledTime;
    }

    private void Update()
    {
        if (effectActive)
        {
            if (collisionEnabledTimer <= 0f)
            {
                potionCollider.enabled = false;
                effectActive = false;
            }
            collisionEnabledTimer -= Time.deltaTime;
        }
    }
}

