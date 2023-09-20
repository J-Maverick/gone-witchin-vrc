
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FlashbangTrigger : UdonSharpBehaviour
{
    public Flashbang flashbang = null;

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (flashbang != null) flashbang.Flash(player);
    }
}
