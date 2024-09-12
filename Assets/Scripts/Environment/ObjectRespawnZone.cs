
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class ObjectRespawnZone : UdonSharpBehaviour
{
    public VRCObjectPool fishPool;
    
    public void OnTriggerEnter(Collider other) {
        Debug.LogFormat("{0}: Trigger entered by {1}", name, other.name);
        if (!other.isTrigger){
            if (other.gameObject.layer == 24 && Networking.GetOwner(other.gameObject).isLocal)
            {
                VRCPickup pickup = other.GetComponent<VRCPickup>();
                if (!pickup.IsHeld) {
                    fishPool.Return(other.gameObject);
                }
            }

            BottleCollision collision = other.GetComponent<BottleCollision>();
            if (collision != null) {
                if (collision.bottle.spawner != null) {
                    collision.Despawn();
                }
                else {
                    collision.Respawn();
                }
                collision.Respawn();
            }
            else {
                VRCObjectSync sync = other.GetComponent<VRCObjectSync>();
                
                if (sync != null) {
                    sync.Respawn();
                }
            }
        }
    }
}
