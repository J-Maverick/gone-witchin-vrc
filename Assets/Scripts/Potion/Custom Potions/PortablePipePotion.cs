
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class PortablePipePotion : ShatterEffect
{
    public VRCObjectPool pool;

    public override void OnShatter()
    {
        if (Networking.GetOwner(gameObject).isLocal) {
            // This may desync
            Networking.SetOwner(Networking.LocalPlayer, pool.gameObject);
            SpawnPipe();
        }
    }

    public void SpawnPipe() {
        GameObject pipe = pool.TryToSpawn();
        Networking.SetOwner(Networking.LocalPlayer, pipe);
        if (pipe != null) {
            Debug.LogFormat("{0}: Spawned pipe.");
            VRCObjectSync sync = pipe.GetComponent<VRCObjectSync>();
            if (sync != null) {
                sync.FlagDiscontinuity();
            }
            pipe.transform.position = transform.position;
        }
    }

    //     public override void OnShatter()
    // {
    //     if (Networking.GetOwner(gameObject).isLocal) {
    //         // This may desync
    //         SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SpawnPipe));
    //     }
    // }

    // public void SpawnPipe() {
    //     if (Networking.GetOwner(pool.gameObject).isLocal) {
    //         SendCustomEventDelayedSeconds(nameof(DelayedSpawn), 1f);
    //     }
    // }

    // public void DelayedSpawn() {
    //     GameObject pipe = pool.TryToSpawn();
    //     Networking.SetOwner(Networking.LocalPlayer, pipe);
    //     if (pipe != null) {
    //         Debug.LogFormat("{0}: Spawned pipe.");
    //         pipe.transform.position = transform.position;
    //     }
    // }
}

