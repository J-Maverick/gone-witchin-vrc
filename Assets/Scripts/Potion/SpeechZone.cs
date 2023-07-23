
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;

public class SpeechZone : UdonSharpBehaviour
{
    public DataList playerList = new DataList(){};
    public DataToken[] players;
    public bool localPlayerInZone = false;
    public SpeechZoneHandler speechZoneHandler = null;
    
    // private bool zoneActive = true;

    private float defaultVoiceNear = 0f;
    private float defaultVoiceFar = 25f;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal) {
            localPlayerInZone = true;
            speechZoneHandler.localPlayerListening = true;
            
            Debug.LogFormat("{0}: Local player entered speech zone", name);
        }
        playerList.Add(player.playerId);
        players = playerList.ToArray();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal) {
            localPlayerInZone = false;
            speechZoneHandler.localPlayerListening = false;
            speechZoneHandler.ResetAllPlayerVoices();
            Debug.LogFormat("{0}: Local player left speech zone", name);
        }
        playerList.Remove(player.playerId);
        players = playerList.ToArray();
        ResetPlayerVoice(player);
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (playerList.Contains(player.playerId)) {
            playerList.Remove(player.playerId);
            players = playerList.ToArray();
        }
    }

    private void SetPlayerVoice(VRCPlayerApi player, float nearDistance, float farDistance) {
        player.SetVoiceDistanceNear(nearDistance);
        player.SetVoiceDistanceFar(farDistance);
    }

    private void ResetPlayerVoice(VRCPlayerApi player) {      
        SetPlayerVoice(player, defaultVoiceNear,defaultVoiceFar);      
    }

    public void SetPlayerVoices() {
        foreach (int id in players) {
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(id);
            if (player != null) {
                float distance = (player.GetPosition() - Networking.LocalPlayer.GetPosition()).magnitude - speechZoneHandler.localPlayerVolumeDistance;
                SetPlayerVoice(player, distance, distance + defaultVoiceFar);
            }
        }
    }

    public void ResetPlayerVoices(VRCPlayerApi[] allPlayers) {
        foreach (VRCPlayerApi player in allPlayers) {
            if (player != null) {
                ResetPlayerVoice(player);
            }
        }
    }

    void Update() {
        if (speechZoneHandler.localPlayerListening && !localPlayerInZone && Time.frameCount % 20 == 0) {
            SetPlayerVoices();
        }
    }
}
