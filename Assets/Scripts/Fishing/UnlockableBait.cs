
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UnlockableBait : UdonSharpBehaviour
{
    public CollectibleBait[] baitsToUnlock;
    public bool unlocked = false;
    
    public void Activate()
    {
        if (unlocked) return;
        foreach (CollectibleBait bait in baitsToUnlock)
        {
            if (Networking.GetOwner(bait.gameObject).isLocal)
            {
                bait.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(CollectibleBait.EnableCollectible));
            }
        }
        unlocked = true;
    }
}
