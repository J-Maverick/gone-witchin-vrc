
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OwnershipTest : UdonSharpBehaviour
{
    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        Debug.LogFormat("{0}: Ownership transferred to {1} -- local: {2}", name, player.displayName, player.isLocal);
    }
}
