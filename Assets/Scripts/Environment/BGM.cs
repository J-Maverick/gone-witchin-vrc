
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BGM : UdonSharpBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    [UdonSynced] public int currentClipIndex = 0;
    [UdonSynced] public bool isPlaying = false;
    public bool isCycleStarted = false;

    private int randomizationDelay = 240;
    public int randomPlayDelayMin = 600;
    public int randomPlayDelayMax = 1200;

    public void Start()
    {
        if (Networking.LocalPlayer.isMaster) {
            currentClipIndex = Random.Range(0, audioClips.Length);
            PlayAudio();
        }
    }

    public void PlayAudio()
    {
        isCycleStarted = true;
        audioSource.clip = audioClips[currentClipIndex];
        if (!audioSource.isPlaying) {
            audioSource.Play();
        }
        if (Networking.LocalPlayer.isMaster) {
            isPlaying = true;
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(RandomizeAudio), randomizationDelay);
            SendCustomEventDelayedSeconds(nameof(PlayRandomAudio), Random.Range(randomPlayDelayMin, randomPlayDelayMax));
        }
    }

    public void RandomizeAudio() {
        currentClipIndex = Random.Range(0, audioClips.Length);
        isPlaying = false;
        RequestSerialization();
    }

    public void PlayRandomAudio() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayAudio));
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal && !player.isMaster) {
            if (isPlaying) {
                PlayAudio();
            }
        }
    }

    public void DisableBGM()
    {
        audioSource.mute = true;
    }

    public void EnableBGM()
    {
        audioSource.mute = false;
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        Debug.LogFormat("{0}: Ownership transferred to {1}", gameObject.name, player.displayName);
        if (player.isLocal && !isCycleStarted) {
            isCycleStarted = true;
            SendCustomEventDelayedSeconds(nameof(RandomizeAudio), randomizationDelay);
            SendCustomEventDelayedSeconds(nameof(PlayAudio), Random.Range(randomPlayDelayMin, randomPlayDelayMax));
        }
    }

}
