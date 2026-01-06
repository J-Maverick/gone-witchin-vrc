
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
    public ReelAngleAccumulator reel = null;
    public WheelAngleAccumulator wheel = null;
    public Lever lever = null;
    public GameObject[] ownershipItems;

    public override void OnDrop()
    {
        dropped = true;
        isHeld = false;
        if (tank != null && Networking.GetOwner(gameObject).isLocal)
        {
            tank.Sync();
            tank.lever.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Lever.Sleep));
        }
        if (lever != null && Networking.GetOwner(gameObject).isLocal) 
        {
            lever.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Lever.Sleep));
        }
        if (reel != null && Networking.GetOwner(gameObject).isLocal) 
        {
            reel.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(reel.Drop));
        }
        if (wheel != null && Networking.GetOwner(gameObject).isLocal) 
        {
            wheel.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(wheel.Drop));
        }
    }

    public override void OnPickup()
    {
        isHeld = true;
        if (tank != null && Networking.GetOwner(gameObject).isLocal) {
            tank.lever.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Lever.WakeUp));
        }
        if (lever != null && Networking.GetOwner(gameObject).isLocal) {
            lever.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Lever.WakeUp));
        }
        if (wheel != null && Networking.GetOwner(gameObject).isLocal) 
        {
            wheel.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(wheel.PickUp));
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
