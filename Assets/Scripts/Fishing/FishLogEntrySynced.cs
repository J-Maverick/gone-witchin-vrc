
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class FishLogEntrySynced : UdonSharpBehaviour
{
    public FishData fishData;
    public FishLogSynced fishLog;
    [UdonSynced, FieldChangeCallback(nameof(largestCaught))] 
    public float _largestCaught = 0f;
    [UdonSynced, FieldChangeCallback(nameof(smallestCaught))] 
    public float _smallestCaught = -1f;
    
    public float largestCaught {
        set {
            if (value != _largestCaught) {
                _largestCaught = value;
                fishLog.UpdateText();
                SendCustomEventDelayedSeconds(nameof(DelayedUpdateText), 1f);
            }
        }
        get => _largestCaught;
    }
        
    public float smallestCaught {
        set {
            if (value != _smallestCaught) {
                _smallestCaught = value;
                fishLog.UpdateText();
                SendCustomEventDelayedSeconds(nameof(DelayedUpdateText), 1f);
            }
        }
        get => _smallestCaught;
    }
    [UdonSynced] public String largestPlayerName = "";
    [UdonSynced] public String smallestPlayerName = "";

    public void UpdateStats(float size) {
        if (size > largestCaught) {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            largestCaught = size;
            largestPlayerName = Networking.LocalPlayer.displayName;
            RequestSerialization();
        }
        if (size < smallestCaught || smallestCaught < 0f) {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            smallestCaught = size;
            smallestPlayerName = Networking.LocalPlayer.displayName;
            RequestSerialization();
        }
    }

    public void DelayedUpdateText() {
        fishLog.UpdateText();
    }

}
