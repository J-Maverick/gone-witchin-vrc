
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

    private void DisableEffect() {
        potionCollider.enabled = false;
        effectActive = false;
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
}
