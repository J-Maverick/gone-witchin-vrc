
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HandleHandler : UdonSharpBehaviour
{
    public bool dropped = false;
    public bool isHeld = false;
    public ReagentTank tank = null;
    public GameObject tankPour = null;

    public override void OnDrop()
    {
        dropped = true;
        isHeld = false;
        if (tank != null)
        {
            tank.Sync();
            tank.lever.Sleep();
        }
    }

    public override void OnPickup()
    {
        isHeld = true;
        if (tank != null) tank.lever.WakeUp();
        if (Networking.LocalPlayer.IsValid())
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            if (tankPour != null) Networking.SetOwner(Networking.LocalPlayer, tankPour);
            if (tank != null) Networking.SetOwner(Networking.LocalPlayer, tank.gameObject);
        }
    }
}
