
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CorkerSnap : BottleSnap
{
    public Corker corker;
    public override void OnTriggerEnter(Collider other)
    {
        Bottle tempBottle = other.gameObject.GetComponent<Bottle>();
        if (tempBottle != null && bottle == null)
        {
            bottle = tempBottle;
            if (snapTarget != null)
            {
                bottle.gameObject.GetComponent<VRC_Pickup>().Drop();
                bottle.transform.SetPositionAndRotation(snapTarget.position, snapTarget.rotation);
                Rigidbody rb = bottle.gameObject.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            if (Networking.GetOwner(other.gameObject).isLocal) {
                corker.TryActivate(bottle);
            }
            SendCustomEventDelayedSeconds("CheckBottleAlive", checkDelayTime);
        }
    }

    public void ClearBottle() {
        bottle = null;
    }
}
