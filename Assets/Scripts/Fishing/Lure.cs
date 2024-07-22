
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Lure : UdonSharpBehaviour
{
    public FishingPole fishingPole;

    public void OnCollisionEnter(Collision collision)
    {
        Water water = collision.gameObject.GetComponent<Water>();
        if (water != null) {
            fishingPole.SplashDown(water);
        }
        Debug.LogFormat("{0} Collided with: {1}", name, collision.gameObject.name);
    }
}
