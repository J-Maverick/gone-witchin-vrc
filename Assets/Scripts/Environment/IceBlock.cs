
using UdonSharp;
using UnityEngine;
using VRC.Core.Source.Config.Interfaces;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class IceBlock : UdonSharpBehaviour
{
    public VRCObjectPool pool;
    public float despawnTime = 10f;
    public Animator animator;
    public FishingZone zone;

    public void OnEnable()
    {
        Debug.LogFormat("{0}: OnEnable", name);
        if (Networking.GetOwner(gameObject).isLocal) {
            Networking.SetOwner(Networking.LocalPlayer, zone.gameObject);
            zone.Activate();
        }
        SendCustomEventDelayedSeconds(nameof(Despawn), despawnTime);
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (player.isLocal) {
            SendCustomEventDelayedSeconds(nameof(Despawn), despawnTime);
            Networking.SetOwner(Networking.LocalPlayer, zone.gameObject);
        }
    }

    public void DelayedDespawn() {
        Debug.LogFormat("{0}: Delayed despawn", name);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(NetworkDespawn));
    }

    public void NetworkDespawn() {
        Debug.LogFormat("{0}: Network despawn", name);
        pool.Return(gameObject);
    }

    public void Despawn() {
        Debug.LogFormat("{0}: Despawning", name);
        animator.Play("IceDespawn");
        zone.DeActivate();
        SendCustomEventDelayedSeconds(nameof(DelayedDespawn), 1f);
    }
}
