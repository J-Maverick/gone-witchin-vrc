
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HandleHandler : UdonSharpBehaviour
{
    public bool dropped = false;
    public ReagentTank tank;

    public override void OnDrop()
    {
        dropped = true;
        tank.Sync();
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, tank.gameObject);
    }
}
