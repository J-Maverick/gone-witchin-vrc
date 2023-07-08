
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
                Networking.SetOwner(Networking.LocalPlayer, potionPool.gameObject);

                GameObject spawnedPotion = potionPool.TryToSpawnByID(bottle.liquid.ID);
                Networking.SetOwner(Networking.LocalPlayer, spawnedPotion);
                if (spawnedPotion != null) {
                    
                    Debug.LogFormat("{0}: Spawned {1}", name, spawnedPotion.name);
                    spawnedPotion.transform.SetPositionAndRotation(bottle.transform.position, bottle.transform.rotation);
                    BottleSync sync = spawnedPotion.GetComponentInChildren<BottleSync>();
                    sync.SetBottleType(bottle.bottleID);

                    bottle.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Despawn");
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
