﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Corker : UdonSharpBehaviour
{
    public CorkerSnap bottleSnap;
    public GemIndicator indicator;
    public PotionOcean potionPool;

    public void TryActivate(Bottle bottle) {
        if (bottle.GetUdonTypeName() == GetUdonTypeName<ReagentBottle>() && bottle.fillLevel >= 1f) {
            indicator.SetValid();
            if (Networking.GetOwner(bottle.gameObject).isLocal) { 
                // TODO update networking -- this is insufficient for remote players. Need to build "animated" corker system that takes its time to properly spawn
                GameObject spawnedPotion = potionPool.TryToSpawnByID(bottle.liquid.ID);
                if (spawnedPotion != null) {
                    Vector3 spawnPosition = bottle.transform.position;
                    Quaternion spawnRotation = bottle.transform.rotation;
                    Networking.SetOwner(Networking.LocalPlayer, spawnedPotion);
                    Debug.LogFormat("{0}: Spawned {1}", name, spawnedPotion.name);
                    BottleSync sync = spawnedPotion.GetComponentInChildren<BottleSync>();
                    Networking.SetOwner(Networking.LocalPlayer, sync.gameObject);
                    sync.SetBottleType(bottle.bottleID);

                    bottle.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Despawn");
                    spawnedPotion.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
                    bottleSnap.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ClearBottle");
                }
                else {
                    indicator.SetInvalid();
                    Debug.LogFormat("{0}: Failed to spawn {1}", name, bottle.liquid.name);
                }
            }
        }
        else {
            indicator.SetInvalid();
            Debug.LogFormat("{0}: Invalid bottle type or insufficient fill", name);
        }
    }
    
}
