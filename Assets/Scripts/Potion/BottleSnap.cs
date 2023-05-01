
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BottleSnap : UdonSharpBehaviour
{
    Bottle bottle = null;
    public Transform snapTarget = null;

    private void OnTriggerEnter(Collider other)
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (bottle != null && other.gameObject == bottle.gameObject)
        {
            bottle = null;
        }
    }

    public Bottle GetBottle()
    {
        return bottle;
    }

    public bool CheckFill(float fill)
    {
        return bottle.fillLevel == fill;
    }

    public bool CheckMatch(Recipe recipe)
    {
        if (recipe != null)
            return bottle.liquid == recipe.potion;
        else return false;
    }
}
