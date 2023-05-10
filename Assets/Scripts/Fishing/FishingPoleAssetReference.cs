
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishingPoleAssetReference : UdonSharpBehaviour
{
    public FishingPole fishingPole;
    public FishForce fishForce;
    public Lure lure;
    public ReelAngleAccumulator reelAngleAccumulator;
    public HandleHandler handleHandler;
}
