
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
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        transform.rotation = Quaternion.identity;
        
        if (Networking.GetOwner(gameObject).isLocal && drinkEffect != null) drinkEffect.OnDrink();

        // TODO replace with object pool logic
        mesh.enabled = false;
        meshCollider.enabled = false;
        pickup.Drop();
        pickup.pickupable = false;
        if (owner != null && owner.isLocal)
        {
            TriggerRespawn();
            syncObj.RandomizeSoundEffect();
        }
        PlayClip(clips, volume, syncObj.soundEffectIndex);
    }

    public virtual void DrinkEffect()
    {
    
    }
}
