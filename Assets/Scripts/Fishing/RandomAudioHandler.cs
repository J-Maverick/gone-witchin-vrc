
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

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
    public AudioSource audioSource;
    public VRCPlayerApi owner;

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

    public void PlaySlotZero()
    {
        AudioClip[] clips = slotZeroClips;
        if (owner != null && owner.isLocal) RandomizeSlotZero();
        PlayClip(clips, slotZeroVolume, slotZeroSoundIndex);
    }

    public void PlaySlotOne()
    {
        AudioClip[] clips = slotOneClips;
        if (owner != null && owner.isLocal) RandomizeSlotOne();
        PlayClip(clips, slotOneVolume, slotOneSoundIndex);
    }

    public void PlaySlotTwo()
    {
        AudioClip[] clips = slotTwoClips;
        if (owner != null && owner.isLocal) RandomizeSlotTwo();
        PlayClip(clips, slotTwoVolume, slotTwoSoundIndex);
    }

    public void PlaySlotThree()
    {
        AudioClip[] clips = slotThreeClips;
        if (owner != null && owner.isLocal) RandomizeSlotThree();
        PlayClip(clips, slotThreeVolume, slotThreeSoundIndex);
    }

    public void PlaySlotFour()
    {
        AudioClip[] clips = slotFourClips;
        if (owner != null && owner.isLocal) RandomizeSlotFour();
        PlayClip(clips, slotFourVolume, slotFourSoundIndex);
    }

    protected void PlayClip(AudioClip[] clips, float volume, int soundIndex)
    {
        Debug.LogFormat("{0}: Playing clip at volume {1}", name, volume);
        audioSource.clip = clips[soundIndex];
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void RandomizeSounds()
    {
        slotZeroSoundIndex = Random.Range(0, slotZeroClips.Length);
        slotOneSoundIndex = Random.Range(0, slotOneClips.Length);
        slotTwoSoundIndex = Random.Range(0, slotTwoClips.Length);
        slotThreeSoundIndex = Random.Range(0, slotThreeClips.Length);
        slotFourSoundIndex = Random.Range(0, slotFourClips.Length);
        RequestSerialization();
    }

    public void RandomizeSlotZero()
    {
        slotZeroSoundIndex = Random.Range(0, slotZeroClips.Length);
        RequestSerialization();
    }

    public void RandomizeSlotOne()
    {
        slotOneSoundIndex = Random.Range(0, slotOneClips.Length);
        RequestSerialization();
    }

    public void RandomizeSlotTwo()
    {
        slotTwoSoundIndex = Random.Range(0, slotTwoClips.Length);
        RequestSerialization();
    }

    public void RandomizeSlotThree()
    {
        slotThreeSoundIndex = Random.Range(0, slotThreeClips.Length);
        RequestSerialization();
    }

    public void RandomizeSlotFour()
    {
        slotFourSoundIndex = Random.Range(0, slotFourClips.Length);
        RequestSerialization();
    }

}
