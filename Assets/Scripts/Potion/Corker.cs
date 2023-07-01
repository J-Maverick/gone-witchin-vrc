
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Corker : UdonSharpBehaviour
{
    public BottleSnap bottleSnap;
    public GemIndicator indicator;
    public PotionOcean potionPool;

    public void TryActivate(Bottle bottle) {
        if (bottle.GetUdonTypeName() == GetUdonTypeName<ReagentBottle>() && bottle.fillLevel >= 1f) {
            indicator.SetValid();
            if (Networking.GetOwner(bottle.gameObject).isLocal) {
                GameObject spawnedPotion = potionPool.TryToSpawnByID(bottle.liquid.ID);
                if (spawnedPotion != null) {
                    
                    Debug.LogFormat("{0}: Spawned {1}", name, spawnedPotion.name);
                    spawnedPotion.transform.SetPositionAndRotation(bottle.transform.position, bottle.transform.rotation);

                    bottle.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Despawn");
                    bottleSnap.bottle = spawnedPotion.GetComponent<Bottle>();
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
