
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Lure : UdonSharpBehaviour
{
    public FishingPole fishingPole;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "LakeWater")
        {
            fishingPole.SplashDown(Location.lake);
        }
        else if (collision.gameObject.name == "CaveWater")
        {
            fishingPole.SplashDown(Location.cave);
        }
    }
}
