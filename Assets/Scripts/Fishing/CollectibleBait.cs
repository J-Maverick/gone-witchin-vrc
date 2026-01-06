
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CollectibleBait : UdonSharpBehaviour
{
    public Bait bait;
    public Renderer baitRenderer;
    public Renderer[] baitRenderers;
    public Light baitLight;
    public Collider baitCollider;
    public bool collectibleOnStartup = false;
    public bool collectible = false;
    public float minRespawnTimeMinutes = 5f;
    public float maxRespawnTimeMinutes = 20f;
    public BaitInventory inventory;
    public float delayTime = 0f;
    public bool localPlayerIsOwner = false;
    private bool waiting = false;

    public void Start() {
        // if (Networking.GetOwner(gameObject).isLocal) {
        //     if (collectibleOnStartup) {
        //         SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Spawn));
        //     }
        //     else {
        //         DisableCollectible();
        //         SendCustomEventDelayedSeconds(nameof(DelayedSpawn), GetRandomTime());
        //     }
        // }
        delayTime = Random.Range(0f, 10f);
        collectible = collectibleOnStartup;
        waiting = true;
        SendCustomEventDelayedSeconds(nameof(Initialize), delayTime);
    }

    public void Initialize() {
        waiting = false;
        if (Networking.GetOwner(gameObject).isLocal)
        {
            localPlayerIsOwner = true;
            if (collectible)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(EnableCollectible));
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(DisableCollectible));
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (localPlayerIsOwner && !waiting) {
            waiting = true;
            SendCustomEventDelayedSeconds(nameof(Initialize), delayTime);
        }
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (player.isLocal) {
            waiting = true;
            SendCustomEventDelayedSeconds(nameof(Initialize), delayTime);
        }
    }

    public void DelayedSpawn() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Spawn));
    }

    public void Spawn() {
        EnableCollectible();
    }

    public override void Interact()
    {
        PlayerData.SetInt(bait.name + DataKeys.BaitCount, PlayerData.GetInt(Networking.LocalPlayer, bait.name + DataKeys.BaitCount) + 1);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Use));
    }

    public void Activate()
    {
        EnableCollectible();
    }

    public void EnableCollectible()
    {
        collectible = true;
        baitCollider.enabled = true;
        if (baitRenderer != null)
        {
            baitRenderer.enabled = true;
        }
        if (baitRenderers.Length > 0)
        {
            for (int i = 0; i < baitRenderers.Length; i++)
            {
                baitRenderers[i].enabled = true;
            }
        }
        if (baitLight != null)
        {
            baitLight.enabled = true;
        }
    }

    public void DisableCollectible() {
        Debug.LogFormat("{0}: DisableCollectible", name);
        collectible = false;
        baitCollider.enabled = false;
        if (baitRenderer != null) {
            baitRenderer.enabled = false;
        }
        if (baitRenderers.Length > 0) {
            for (int i = 0; i < baitRenderers.Length; i++) {
                baitRenderers[i].enabled = false;
            }
        }
        if (baitLight != null) {
            baitLight.enabled = false;
        }
    }

    public void Use() {
        inventory.AddBait(bait);
        DisableCollectible();
        if (Networking.GetOwner(gameObject).isLocal) {
            SendCustomEventDelayedSeconds(nameof(DelayedSpawn), GetRandomTime());
        }
    }

    float GetRandomTime() {
        return Random.Range(minRespawnTimeMinutes * 60f, maxRespawnTimeMinutes * 60f);
    }
}
