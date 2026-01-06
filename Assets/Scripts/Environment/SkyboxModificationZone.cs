
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SkyboxModificationZone : UdonSharpBehaviour
{
    public NewSuperDayNightCycle dayNightCycle;
    public Color fogColor;
    public Color skyColor;
    public int locationIndex = 0;
    public Fireflies fireflies;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal) {
            dayNightCycle.SetTempColor(fogColor, skyColor);
            fireflies.SetColor(locationIndex);
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal) {
            dayNightCycle.ResetTempColor();
            fireflies.SetColor(0);
        }
    }
}
