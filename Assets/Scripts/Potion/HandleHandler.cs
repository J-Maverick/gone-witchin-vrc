
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class HandleHandler : UdonSharpBehaviour
{
    public bool dropped = false;
    public bool isHeld = false;
    public ReagentTank tank = null;
    public Lever lever = null;
    public GameObject[] ownershipItems;

    public override void OnDrop()
    {
        dropped = true;
        isHeld = false;
        if (tank != null && Networking.GetOwner(gameObject).isLocal)
        {
            tank.Sync();
            tank.lever.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sleep");
        }
        if (lever != null && Networking.GetOwner(gameObject).isLocal) 
        {
            lever.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sleep");
        }
    }

    public override void OnPickup()
    {
        isHeld = true;
        if (tank != null && Networking.GetOwner(gameObject).isLocal) {
            tank.lever.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "WakeUp");
        }
        if (lever != null && Networking.GetOwner(gameObject).isLocal) {
            lever.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "WakeUp");
        }
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (player.isLocal) {
            foreach (GameObject ownershipObject in ownershipItems) {
                Networking.SetOwner(player, ownershipObject);
            }
        }
    }
}
