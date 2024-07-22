
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class Portal : UdonSharpBehaviour
{
    public Portal targetPortal;
    public Transform spawnTransform;
    public bool active = true;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal && active) {
            targetPortal.active = false;
            // Drop fishing pole -- holy moly do they lag on teleport
            VRC_Pickup lPickup = player.GetPickupInHand(VRC_Pickup.PickupHand.Left);
            VRC_Pickup rPickup = player.GetPickupInHand(VRC_Pickup.PickupHand.Right);
            if (lPickup != null && lPickup.name == "Fishpole") {
                lPickup.Drop();
            }
            if (rPickup != null && rPickup.name == "Fishpole") {
                rPickup.Drop();
            }
            player.TeleportTo(targetPortal.spawnTransform.position, targetPortal.spawnTransform.rotation);
        }
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal) {
            active = true;
        }
    }
}
