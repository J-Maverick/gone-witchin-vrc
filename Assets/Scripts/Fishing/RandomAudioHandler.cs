
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RandomAudioHandler : UdonSharpBehaviour
{
    public AudioClip[] slotZeroClips;
    public float slotZeroVolume = 1f;
    [UdonSynced] public int slotZeroSoundIndex = 0;
    public AudioClip[] slotOneClips;
    public float slotOneVolume = 1f;
    [UdonSynced] public int slotOneSoundIndex = 0;
    public AudioClip[] slotTwoClips;
    public float slotTwoVolume = 1f;
    [UdonSynced] public int slotTwoSoundIndex = 0;
    public AudioClip[] slotThreeClips;
    public float slotThreeVolume = 1f;
    [UdonSynced] public int slotThreeSoundIndex = 0;
    public AudioClip[] slotFourClips;
    public float slotFourVolume = 1f;
    [UdonSynced] public int slotFourSoundIndex = 0;
    public float maxPitch = 1.1f;
    public float minPitch = 0.9f;
    public AudioSource audioSource;
    public VRCPlayerApi owner;
    public bool localOnly = false;

    private void Start()
    {
        owner = Networking.GetOwner(gameObject);

        if (owner != null && owner.isLocal) RandomizeSounds();
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        owner = player;
        if (owner != null)
        {
            RandomizeSounds();
        }
    }


    public void PlaySlotZeroNetworked() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySlotZero));
    }

    public void PlaySlotOneNetworked() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySlotOne));
    }

    public void PlaySlotTwoNetworked() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySlotTwo));
    }

    public void PlaySlotThreeNetworked() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySlotThree));
    }

    public void PlaySlotFourNetworked() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySlotFour));
    }

    public void PlaySlotZero()
    {
        AudioClip[] clips = slotZeroClips;
        if ((owner != null && owner.isLocal) || localOnly) RandomizeSlotZero();
        PlayClip(clips, slotZeroVolume, slotZeroSoundIndex);
    }

    public void PlaySlotOne()
    {
        AudioClip[] clips = slotOneClips;
        if ((owner != null && owner.isLocal) || localOnly) RandomizeSlotOne();
        PlayClip(clips, slotOneVolume, slotOneSoundIndex);
    }

    public void PlaySlotTwo()
    {
        AudioClip[] clips = slotTwoClips;
        if ((owner != null && owner.isLocal) || localOnly) RandomizeSlotTwo();
        PlayClip(clips, slotTwoVolume, slotTwoSoundIndex);
    }

    public void PlaySlotThree()
    {
        AudioClip[] clips = slotThreeClips;
        if ((owner != null && owner.isLocal) || localOnly) RandomizeSlotThree();
        PlayClip(clips, slotThreeVolume, slotThreeSoundIndex);
    }

    public void PlaySlotFour()
    {
        AudioClip[] clips = slotFourClips;
        if ((owner != null && owner.isLocal) || localOnly) RandomizeSlotFour();
        PlayClip(clips, slotFourVolume, slotFourSoundIndex);
    }

    protected void PlayClip(AudioClip[] clips, float volume, int soundIndex)
    {
        Debug.LogFormat("{0}: Playing clip at volume {1}", name, volume);
        audioSource.enabled = true;
        audioSource.clip = clips[soundIndex];
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        if (Vector3.Distance(Networking.LocalPlayer.GetPosition(), transform.position) < audioSource.maxDistance)
        {
            audioSource.Play();
        }
        SendCustomEventDelayedSeconds(nameof(TryDisableAudioSource), audioSource.clip.length + 0.1f);
    }

    public void TryDisableAudioSource()
    {
        if (audioSource.isPlaying) return;
        audioSource.enabled = false;
    }

    public void RandomizeSounds()
    {
        slotZeroSoundIndex = Random.Range(0, slotZeroClips.Length);
        slotOneSoundIndex = Random.Range(0, slotOneClips.Length);
        slotTwoSoundIndex = Random.Range(0, slotTwoClips.Length);
        slotThreeSoundIndex = Random.Range(0, slotThreeClips.Length);
        slotFourSoundIndex = Random.Range(0, slotFourClips.Length);
        if (!localOnly) {
            RequestSerialization();
        }
    }

    public void RandomizeSlotZero()
    {
        slotZeroSoundIndex = Random.Range(0, slotZeroClips.Length);
        if (!localOnly) {
            RequestSerialization();
        }
    }

    public void RandomizeSlotOne()
    {
        slotOneSoundIndex = Random.Range(0, slotOneClips.Length);
        if (!localOnly) {
            RequestSerialization();
        }
    }

    public void RandomizeSlotTwo()
    {
        slotTwoSoundIndex = Random.Range(0, slotTwoClips.Length);
        if (!localOnly) {
            RequestSerialization();
        }
    }

    public void RandomizeSlotThree()
    {
        slotThreeSoundIndex = Random.Range(0, slotThreeClips.Length);
        if (!localOnly) {
            RequestSerialization();
        }
    }

    public void RandomizeSlotFour()
    {
        slotFourSoundIndex = Random.Range(0, slotFourClips.Length);
        if (!localOnly) {
            RequestSerialization();
        }
    }

}
