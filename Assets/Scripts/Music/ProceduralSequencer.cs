
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ProceduralSequencer : UdonSharpBehaviour
{
    public SequencerChannel[] channels;
    public float bpm = 120f;
    public int timeSignature = 4;
    public int beatInterval = 2;

    void Start()
    {
        BeatTimer();
    }

    public void BeatTimer()
    {
        foreach(SequencerChannel channel in channels) {
            channel.FireChannel();
        }
        SendCustomEventDelayedSeconds(nameof(BeatTimer), 60f / bpm * timeSignature / beatInterval);
    }

    public void EnableChannel(int channelIndex) {
        if (channelIndex >= 0 && channelIndex < channels.Length) {
            channels[channelIndex].EnableChannel();
        }
    }
    public void DisableChannel(int channelIndex) {
        if (channelIndex >= 0 && channelIndex < channels.Length) {
            channels[channelIndex].DisableChannel();
        }
    }
}
