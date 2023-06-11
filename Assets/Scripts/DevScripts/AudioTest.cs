
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AudioTest : UdonSharpBehaviour
{
    private float defaultVoiceGain = 15f;
    private float defaultVoiceNear = 0f;
    private float defaultVoiceFar = 25f;
    public bool localPlayerPresent = false;

    private void SetPlayerVoice(VRCPlayerApi player, 
    float gain, 
    float nearDistance,
    float farDistance
    // float volumetricRadius,
    //  bool lowPass
     ) {
        player.SetVoiceGain(gain);
        player.SetVoiceDistanceNear(nearDistance);
        player.SetVoiceDistanceFar(farDistance);
        // player.SetVoiceVolumetricRadius(volumetricRadius);
        // player.SetVoiceLowpass(lowPass);
    }

    private void ResetPlayerVoice(VRCPlayerApi player) {      
        SetPlayerVoice(player, 
        defaultVoiceGain, 
        defaultVoiceNear,
        defaultVoiceFar
        // defaultVoiceVolumetricRadius, 
        // defaultLowPass
        );      
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        SetPlayerVoice(player, 
        0f, 
        0f,
        100000f
        // defaultVoiceVolumetricRadius, 
        // false
        );
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        ResetPlayerVoice(player);
    }
}
