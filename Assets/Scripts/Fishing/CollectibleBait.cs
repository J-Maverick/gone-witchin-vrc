
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CollectibleBait : UdonSharpBehaviour
{
    public Bait bait;
    public Renderer baitRenderer;
    public Collider baitCollider;
    public bool collectibleOnStartup = false;
    [UdonSynced] public bool collectible = false;
    public float minRespawnTimeMinutes = 5f;
    public float maxRespawnTimeMinutes = 20f;
    public BaitInventory inventory;

    public void Start() {
        if (Networking.GetOwner(gameObject).isLocal) {
            if (collectibleOnStartup) {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Spawn");
            }
            else {
                SendCustomEventDelayedSeconds("DelayedSpawn", GetRandomTime());
            }
        }
    }

    public override void OnDeserialization()
    {
        if (collectible) {
            Spawn();
        }
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (player.isLocal) {
            if (collectible) {
                EnableCollectible();
            }
            else {
                DisableCollectible();
            }
        }
    }

    public void DelayedSpawn() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Spawn");
    }

    public void Spawn() {
        EnableCollectible();
        RequestSerialization();
    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Use");
    }

    public void EnableCollectible() {
        collectible = true;
        baitCollider.enabled = true;
        baitRenderer.enabled = true;
    }

    public void DisableCollectible() {
        collectible = false;
        baitCollider.enabled = false;
        baitRenderer.enabled = false;
    }

    public void Use() {
        inventory.AddBait(bait);
        DisableCollectible();
        if (Networking.GetOwner(gameObject).isLocal) {
            SendCustomEventDelayedSeconds("DelayedSpawn", GetRandomTime());
        }
    }

    float GetRandomTime() {
        return Random.Range(minRespawnTimeMinutes * 60f, maxRespawnTimeMinutes * 60f);
    }
}
