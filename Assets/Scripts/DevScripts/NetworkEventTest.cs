
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NetworkEventTest : UdonSharpBehaviour
{
    void Awake()
    {
        RecursiveEvent();
    }

    void RecursiveEvent() {
        SendCustomEventDelayedSeconds("RecursiveEvent", Random.Range(60,240));
    }
}
