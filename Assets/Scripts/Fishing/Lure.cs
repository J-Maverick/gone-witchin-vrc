
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Lure : UdonSharpBehaviour
{
    public FishingPole fishingPole;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 26) return;
        if (!fishingPole.casted) return;
        Water water = other.gameObject.GetComponent<Water>();
        if (water != null) {
            fishingPole.SplashDown(water);
        }
        Debug.LogFormat("{0} Collided with: {1}", name, other.gameObject.name);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != 26) return;
        if (!fishingPole.inWater) return;
        float verticalDistance = Mathf.Min(transform.position.y - other.transform.position.y, 0f);
        fishingPole.fishForce.ApplyBouyantForce(verticalDistance);
    }
}
