
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RespawnZone : UdonSharpBehaviour
{

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal) player.Respawn();
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player.isLocal) player.Respawn();
    }
}
