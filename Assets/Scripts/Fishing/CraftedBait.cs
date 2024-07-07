
using System.Data.SqlClient;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class CraftedBait : UdonSharpBehaviour
{
    public Bait bait;
    public BaitInventory inventory;
    public VRCObjectPool pool;

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Use");
    }

    public void Use() {
        inventory.AddBait(bait);
        pool.Return(gameObject);
    }
}
