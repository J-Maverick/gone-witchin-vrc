
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DrinkablePotion : BottleCollision
{
    public DrinkEffect drinkEffect = null;

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Drink");
    }

    public void Drink()
    {
        audioSource.maxDistance = 25f;
        AudioClip[] clips = soundEffectClips;
        float volume = soundEffectVolume;
        
        if (Networking.GetOwner(gameObject).isLocal && drinkEffect != null) drinkEffect.OnDrink();

        Broken();
        if (owner != null && owner.isLocal)
        {
            if (bottle.spawner != null) {
                SendCustomEventDelayedSeconds(nameof(DelayedDespawn), respawnTime);
            }
            else {
                SendCustomEventDelayedSeconds(nameof(DelayedRespawn), respawnTime);
            }
            syncObj.RandomizeSoundEffect();
            syncObj.SetBroken(isBroken);
        }
        PlayClip(clips, volume, syncObj.soundEffectIndex);
    }

    public virtual void DrinkEffect()
    {
    
    }
}
