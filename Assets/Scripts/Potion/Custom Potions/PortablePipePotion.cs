
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
            GameObject pipe = pool.TryToSpawn();
            if (pipe != null) {
                pipe.transform.position = transform.position;
            }
        }
    }
}

