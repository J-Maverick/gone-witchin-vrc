
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishForceDirectionChanger : UdonSharpBehaviour
{
    public FishingPole fishingPole;
    public FishForce fishForce;
    public void OnTriggerEnter(Collider other)
    {
        if (fishingPole.fishOn)
        {
            Debug.LogFormat("{0}: Fish hit {1}", name, other.name);
            fishForce.RandomReverseDirection();
        }
    }
}
