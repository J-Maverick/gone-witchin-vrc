
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class FishForceSync : UdonSharpBehaviour
{
    public Bait bait = null;
    public Hook hook;
    public BaitInventory baitInventory;
    [UdonSynced] public int baitIndex = -1;
    [UdonSynced] public int baitUsesRemaining = 0;

    public void AddBait(Bait newBait) {
        if (newBait == null) return;
        // Stacks bait if same bait, else replaces
        if (bait == newBait) {
            baitUsesRemaining += newBait.castsPerBait;
            Debug.LogFormat("{0}: Refreshed Bait {1}, casts remaining: {2}", name, newBait.name, baitUsesRemaining);
        }
        else {
            bait = newBait;
            baitUsesRemaining = newBait.castsPerBait;
            Debug.LogFormat("{0}: Added New Bait {1}, casts remaining: {2}", name, newBait.name, baitUsesRemaining);
        }
        RequestSerialization();
    }

    public override void OnPreSerialization()
    {
        baitIndex = baitInventory.GetBaitIndex(bait);
    }

    public override void OnDeserialization()
    {
        if (!Networking.GetOwner(gameObject).isLocal) {
            bait = baitInventory.GetBaitByIndex(baitIndex);
            if (bait != null) {
                hook.baitMesh.sharedMesh = bait.mesh;
                hook.meshRenderer.material.color = bait.material.color;
            }
            else {
                hook.baitMesh.sharedMesh = null;
            }
        }
    }
}
