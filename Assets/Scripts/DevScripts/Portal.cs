
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class Portal : UdonSharpBehaviour
{
    public Portal targetPortal;
    public Transform spawnTransform;
    public AudioSource teleportSound;
    public bool active = true;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal && active)
        {
            targetPortal.active = false;
            // Drop fishing pole -- holy moly do they lag on teleport
            VRC_Pickup lPickup = player.GetPickupInHand(VRC_Pickup.PickupHand.Left);
            VRC_Pickup rPickup = player.GetPickupInHand(VRC_Pickup.PickupHand.Right);
            if (lPickup != null && lPickup.name == "Fishpole")
            {
                lPickup.Drop();
            }
            if (rPickup != null && rPickup.name == "Fishpole")
            {
                rPickup.Drop();
            }
            player.TeleportTo(targetPortal.spawnTransform.position, targetPortal.spawnTransform.rotation);
            targetPortal.SendCustomEventDelayedSeconds(nameof(PlayAudio), 0.1f);
        }
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            active = true;
        }
    }

    public void PlayAudio()
    {
        teleportSound.enabled = true;
        SendCustomEventDelayedSeconds(nameof(DisableAudio), teleportSound.clip.length + 0.1f);
    }

    public void DisableAudio()
    {
        if (teleportSound.isPlaying)
        {
            SendCustomEventDelayedSeconds(nameof(DisableAudio), teleportSound.clip.length + 0.1f);
            return;
        }
        teleportSound.enabled = false;
    }
}
