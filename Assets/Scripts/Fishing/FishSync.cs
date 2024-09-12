
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class FishSync : UdonSharpBehaviour
{
    public Fish fish;
    [UdonSynced] public int fishIDSync;
    [UdonSynced] public float weight = 1f;
    [UdonSynced] public float size = 1f;
    [UdonSynced] public float exhaustion = 1f;
    [UdonSynced] public int stateIDSync = 0;
    [UdonSynced] public bool pickupEnabledSync = false;

    public float intervalTime = 3f;
    private int nJoinSyncs = 10;
    public int joinSyncCounter = 0;

    public override void OnPreSerialization()
    {
        if (Networking.GetOwner(gameObject).isLocal)
        {
            fishIDSync = fish.fishID;
            weight = fish.weight;
            size = fish.size;
            exhaustion = fish.exhaustion;
            stateIDSync = (int)fish.state;
            pickupEnabledSync = fish.pickupEnabled;
            //RequestSerialization();
        }
    }

    public void SendPickup() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FishPickup));
    }
    
    public void FishPickup() {
        fish.OnPickup();
    }

    public override void OnDeserialization()
    {
        if (!Networking.GetOwner(gameObject).isLocal)
        {
            Debug.Log("Deserializing fish data...");
            fish.state = (FishState)stateIDSync;
            if (fish.pickupEnabled != pickupEnabledSync)
            {
                switch (pickupEnabledSync)
                {
                    case true:
                        fish.EnablePickup();
                        break;
                    case false:
                        fish.DisablePickup();
                        break;
                }
            }
            if (fish.fishID != fishIDSync)
            {
                if (fishIDSync == -1) fish.Reset();
                else
                {
                    fish.fishData = fish.fishDataPool.GetFishByID(fishIDSync);
                    fish.fishID = fishIDSync;
                    if (fish.fishData != null) fish.SetFishSizeProperties();
                    else Debug.LogFormat("{0}: fishData updated with null from fishID: {1}", name, fish.fishID);
                }
            }
            else if (fish.fishData != null && fish.meshRenderer.sharedMesh != fish.fishData.mesh) fish.SetFishSizeProperties();
            SetFishPropertiesFromDeserialization();
            SetStateFromDeserialization();
        }
    }

    public void SetStateFromDeserialization()
    {
        if (fish.state == FishState.biting)
        {
            fish.animator.SetBool("Bite", true);
            fish.animator.SetFloat("SwimSpeed", fish.defaultSwimSpeed * fish.fishData.forceMultiplier);
        }
        else if (fish.state == FishState.fighting || fish.state == FishState.catchable)
        {
            fish.animator.SetBool("Bite", true);
            fish.animator.SetFloat("SwimSpeed", fish.defaultSwimSpeed * fish.fishData.forceMultiplier * exhaustion);
        }
        else if (fish.state == FishState.catching || fish.state == FishState.caught)
        {
            fish.animator.SetFloat("SwimSpeed", fish.defaultSwimSpeed * fish.fishData.forceMultiplier * exhaustion);
            fish.animator.SetBool("Bite", true);
            fish.animator.SetBool("IsCaught", true);
            fish.material.SetFloat("_WaterLevel", -100f);
        }
        else if (fish.state == FishState.reset || fish.state == FishState.basket)
        {
            fish.animator.SetBool("Bite", false);
            fish.animator.SetBool("IsCaught", false);
        }
    }

    public void SetFishPropertiesFromDeserialization()
    {
        fish.weight = weight;
        fish.size = size;
        fish.exhaustion = exhaustion;
    }

    public void Sync()
    {
        RequestSerialization();
    }

    public void JoinSync() {
        if (joinSyncCounter < nJoinSyncs) {
            SendCustomEventDelayedSeconds("JoinSync", intervalTime);
            Sync();
            joinSyncCounter++;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        joinSyncCounter = 0;
        JoinSync();
    }
}
