
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SunExclusionZone : UdonSharpBehaviour
{
    public NewSuperDayNightCycle dayNightCycle;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal) {
            dayNightCycle.DisableSun();
        }
    }

    
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal) {
            dayNightCycle.EnableSun();
        }
    }
}
