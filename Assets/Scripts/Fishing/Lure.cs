
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Lure : UdonSharpBehaviour
{
    public FishingPole fishingPole;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Water")
        {
            fishingPole.SplashDown();
        }
    }
}
