
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SequencerChannel : UdonSharpBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] gestures;
    public AudioClip[] restGestures;
    public bool channelActive = false;
    public int interruptChance = 20;
    public int interruptRestQuotient = 4;
    public int nGestures = 4;
    public int nRests = 2;
    public int gestureCount = 0;
    public int restCount = 0;
    public int gestureIndex = -2;

    public void EnableChannel() {
        channelActive = true;
    }

    public void DisableChannel() {
        channelActive = false;
    }

    public void PickRandomClip()
    {
        if (gestures.Length == 0) return;
        int randomIndex = Random.Range(0, gestures.Length);
        if (randomIndex == gestureIndex && gestures.Length > 1)
        {
            PickRandomClip();
        }
        else {
            gestureIndex = randomIndex;
            gestureCount++;
            audioSource.clip = gestures[randomIndex];
            audioSource.Play(0);
        }
    }

    public void PickRandomRestClip()
    {
        if (restGestures.Length == 0) return;
        gestureIndex = -1;
        restCount++;
        audioSource.clip = restGestures[Random.Range(0, restGestures.Length)];
        audioSource.Play(0);
    }

    public void FireChannel() {
        if (channelActive)
        {
            if (!audioSource.enabled)
            {
                audioSource.enabled = true;
            }

            if (gestureCount >= nGestures)
            {
                if (!audioSource.isPlaying)
                {
                    PickRandomRestClip();
                }
                if (restCount >= nRests)
                {
                    gestureCount = 0;
                    restCount = 0;
                }
            }

            int rng = Random.Range(0, 100);
            int interruptValue = gestureCount < nGestures ? interruptChance : interruptChance / interruptRestQuotient;
            if (rng < interruptValue || !audioSource.isPlaying)
            {
                if (restCount > 0) {
                    gestureCount = 0;
                    restCount = 0;
                }
                PickRandomClip();
            }
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.enabled = false;
        }
    }
}
