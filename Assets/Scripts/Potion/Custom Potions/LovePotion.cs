
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LovePotion : ShatterEffect
{
    public PostProcessingProfileManager manager;
    public BottleCollision bottleCollision;
    public SphereCollider potionCollider;

    public float collisionEnabledTime = 2f;
    public float collisionEnabledTimer = 0f;

    bool soundPlayed = false;
    bool soundPlayedLocal = false;
    bool effectActive = false;

    public override void OnShatter()
    {
        effectActive = true;
        potionCollider.enabled = true;
        collisionEnabledTimer = collisionEnabledTime;
    }

    public void ActivateLove(VRCPlayerApi player)
    {
        if (effectActive)
        {
            if (player.isLocal)
            {
                if (!soundPlayedLocal)
                {
                    manager.Love();
                    bottleCollision.PlaySoundEffect();
                    soundPlayedLocal = true;
                }
            }
            else if (!soundPlayed)
            {
                bottleCollision.PlaySoundEffect();
                soundPlayed = true;
            }
        }
    }

    private void Update()
    {
        if (effectActive)
        {
            if (collisionEnabledTimer <= 0f)
            {
                potionCollider.enabled = false;
                soundPlayed = false;
                soundPlayedLocal = false;
                effectActive = false;
            }
            collisionEnabledTimer -= Time.deltaTime;
        }
    }
}
