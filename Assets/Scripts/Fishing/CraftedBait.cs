
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
    public bool collectible = true;

    public override void OnSpawn()
    {
        collectible = true;
    }

    public override void Interact()
    {
        if (collectible) {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Use");
        }
    }

    public void Use() {
        inventory.AddBait(bait);
        collectible = false;
        if (Networking.GetOwner(gameObject).isLocal) {
            pool.Return(gameObject);
        }
    }
}
