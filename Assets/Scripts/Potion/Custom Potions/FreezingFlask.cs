
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class FreezingFlask : ShatterEffect
{
    public VRCObjectPool pool;

    public override void OnShatter()
    {
        if (Networking.GetOwner(gameObject).isLocal) {
            Networking.SetOwner(Networking.LocalPlayer, pool.gameObject);
            Spawn();
        }
    }

    public void Spawn() {
        GameObject ice = pool.TryToSpawn();
        if (ice != null) {
            Debug.LogFormat("{0}: Spawned ice.", name);
            Networking.SetOwner(Networking.LocalPlayer, ice);
            VRCObjectSync sync = ice.GetComponent<VRCObjectSync>();
            if (sync != null) {
                sync.FlagDiscontinuity();
            }
            ice.transform.position = transform.position;
        }
        else {
            Debug.LogFormat("{0}: Failed to spawn ice.", name);
        }
    }

}
