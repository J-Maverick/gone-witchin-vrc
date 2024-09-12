
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PotionInventorySnap : UdonSharpBehaviour
{
    public PotionInventoryButton button;

    public void OnTriggerEnter(Collider other) {
        Bottle bottle = other.GetComponent<Bottle>();
        if (bottle != null && bottle.liquid != null) {
            button.SendBottle(bottle);
        }
    }

    public void OnTriggerExit(Collider other) {
        Bottle bottle = other.GetComponent<Bottle>();
        if (bottle != null && bottle.liquid != null) {
            button.ClearBottle(bottle);
        }
    }
}
