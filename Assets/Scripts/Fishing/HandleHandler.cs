
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HandleHandler : UdonSharpBehaviour
{
    public bool dropped = false;
    public ReagentTank tank = null;
    public GameObject tankPour = null;

    public override void OnDrop()
    {
        dropped = true;
        if (tank != null) tank.Sync();
    }

    public override void OnPickup()
    {
        if (Networking.LocalPlayer.IsValid())
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, tankPour);
            if (tank != null) Networking.SetOwner(Networking.LocalPlayer, tank.gameObject);
        }
    }
}
