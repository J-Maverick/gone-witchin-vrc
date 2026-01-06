
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RespawnZone : UdonSharpBehaviour
{
    public RespawnFollower respawnFollower;
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal) respawnFollower.RespawnPlayer();
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player.isLocal) respawnFollower.RespawnPlayer();
    }
}
