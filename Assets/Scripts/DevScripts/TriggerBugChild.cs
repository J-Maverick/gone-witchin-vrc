
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TriggerBugChild : TriggerBugParent
{
    public override void DoNothing()
    {
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        Debug.LogFormat("{0}: Player {1} [2] is in trigger.", name, player.displayName, player.playerId);
    }
}
