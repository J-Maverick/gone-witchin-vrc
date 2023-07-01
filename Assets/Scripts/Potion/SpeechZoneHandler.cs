
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpeechZoneHandler : UdonSharpBehaviour
{
    public bool localPlayerListening = false;
    public bool delayTriggered = false;
    private float delayTime = 0.25f;

    public SpeechZone[] speechZones;
    public float localPlayerVolumeDistance = 0f;

    public void SetLocalPlayerVolumeDistance() {
        foreach (SpeechZone zone in speechZones) {
            if (zone.localPlayerInZone) {
                localPlayerVolumeDistance = (Networking.LocalPlayer.GetPosition() - zone.transform.position).magnitude;
            }
        }
        delayTriggered = false;
    }

    void Update() {
        if (localPlayerListening && !delayTriggered) {
            SendCustomEventDelayedSeconds("SetLocalPlayerVolumeDistance", delayTime);
            delayTriggered = true;
        }
    }

    public void ResetAllPlayerVoices() {
        VRCPlayerApi[] allPlayers = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(allPlayers);

        foreach (SpeechZone zone in speechZones) {
            zone.ResetPlayerVoices(allPlayers);
        }
    }
}
