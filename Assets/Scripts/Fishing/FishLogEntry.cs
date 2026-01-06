
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishLogEntry : UdonSharpBehaviour
{
    public FishData fishData;
    public float largestCaught = 0f;
    public float smallestCaught = -1f;
}
