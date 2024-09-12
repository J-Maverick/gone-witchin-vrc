
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class FishingPolePool : UdonSharpBehaviour
{
    public VRCObjectPool objectPool;
    public FishingPole[] fishingPoles;

    public void Start()
    {
        if (Networking.LocalPlayer.isMaster) {
            Debug.LogFormat("{0}: Start", name);
            GameObject poleObj = objectPool.TryToSpawn();
            if (poleObj != null) {
                Debug.LogFormat("{0}: Spawned", name);
                FishingPole fishingPole = poleObj.GetComponentInChildren<FishingPole>();
                fishingPole.assignedPlayer = Networking.LocalPlayer;
                fishingPole.pickup.pickupable = true;
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.isMaster && !player.isLocal) {
            Debug.LogFormat("{0}: Spawning pole for {1} [{2}]", name, player.displayName, player.playerId);
            GameObject poleObj = objectPool.TryToSpawn();
            if (poleObj != null) {
                FishingPole fishingPole = poleObj.GetComponentInChildren<FishingPole>();
                fishingPole.assignedPlayer = player;
                Networking.SetOwner(player, fishingPole.gameObject);
                foreach (GameObject obj in fishingPole.ownedObjects) { 
                    Networking.SetOwner(player, obj);
                }
                if (fishingPole.fishForce.fish != null) Networking.SetOwner(player, fishingPole.fishForce.fish.gameObject);
                fishingPole.SendCustomEventDelayedSeconds(nameof(FishingPole.DelayedPickup), 1f);
                fishingPole.SendCustomEventDelayedSeconds(nameof(FishingPole.DelayedPickup), 5f);
                fishingPole.SendCustomEventDelayedSeconds(nameof(FishingPole.DelayedPickup), 10f);
                fishingPole.SendCustomEventDelayedSeconds(nameof(FishingPole.DelayedPickup), 20f);
            }
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (Networking.Master.isLocal) {
            foreach (FishingPole fishingPole in fishingPoles)
            {
                if (fishingPole.assignedPlayer == player)
                {
                    fishingPole.assignedPlayer = null;
                    objectPool.Return(fishingPole.transform.parent.parent.gameObject);
                }
            }
        }
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (player.isLocal) {
            foreach (FishingPole fishingPole in fishingPoles)
            {
                if (fishingPole.gameObject.activeSelf) {
                    fishingPole.assignedPlayer = Networking.GetOwner(fishingPole.gameObject);
                }
                else {
                    fishingPole.assignedPlayer = null;
                }
            }
        }
    }

    public void SummonRod(Vector3 position)
    {
        Debug.LogFormat("{0}: SummonRod", name);
        foreach (FishingPole fishingPole in fishingPoles)
        {
            if (fishingPole.gameObject.activeSelf && Networking.GetOwner(fishingPole.gameObject).isLocal)
            {
                fishingPole.Teleport(position);
            }
        }
    }
}
