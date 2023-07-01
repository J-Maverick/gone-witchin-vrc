
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
    public GameObject tankPour = null;

    public GameObject spawnPool = null;

    public override void OnDrop()
    {
        dropped = true;
        isHeld = false;
        if (tank != null)
        {
            tank.Sync();
            tank.lever.Sleep();
        }
        if (lever != null) lever.Sleep();
    }

    public override void OnPickup()
    {
        isHeld = true;
        if (tank != null) tank.lever.WakeUp();
        if (lever != null) lever.WakeUp();
        if (Networking.LocalPlayer.IsValid())
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            if (tankPour != null) Networking.SetOwner(Networking.LocalPlayer, tankPour);
            if (tank != null) Networking.SetOwner(Networking.LocalPlayer, tank.gameObject);
            if (spawnPool != null) Networking.SetOwner(Networking.LocalPlayer, spawnPool);
            if (lever != null) Networking.SetOwner(Networking.LocalPlayer, lever.gameObject);
        }
    }
}
