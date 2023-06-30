
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
                fishPool.Return(other.gameObject);
            }
            VRCObjectSync sync = other.GetComponent<VRCObjectSync>();
            
            if (sync != null) {
                sync.Respawn();
            }
        }
    }
}
