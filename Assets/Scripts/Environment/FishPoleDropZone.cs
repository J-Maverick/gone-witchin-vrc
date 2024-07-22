
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishPoleDropZone : UdonSharpBehaviour
{
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal) {
                VRC_Pickup lPickup = player.GetPickupInHand(VRC_Pickup.PickupHand.Left);
                VRC_Pickup rPickup = player.GetPickupInHand(VRC_Pickup.PickupHand.Right);
                if (lPickup != null && lPickup.name == "Fishpole") {
                    lPickup.Drop();
                }
                if (rPickup != null && rPickup.name == "Fishpole") {
                    rPickup.Drop();
                }
            }
        }
}
