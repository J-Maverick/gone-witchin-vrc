
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PotionInventoryStorage : UdonSharpBehaviour
{
    public GameObject personalInventoryPage;
    public PotionInventory sharedInventory;
    public PersonalPotionInventory personalInventory;
    public bool storageEnabled = true;


    public void OnTriggerEnter(Collider other) {
        Debug.LogFormat("{0}: OnTriggerEnter", name);
        if (!storageEnabled) return;
        Bottle bottle = other.GetComponent<Bottle>();
        if (bottle == null) return;
        if (!Networking.GetOwner(bottle.gameObject).isLocal) {
            return;
        }
        if (bottle != null && bottle.liquid != null)
        {
            if (!personalInventoryPage.activeSelf)
            {
                sharedInventory.AddToInventory(bottle);
            }
            else
            {
                personalInventory.AddToInventory(bottle);
            }
        }
        TempStorageDisable();
    }

    public void TempStorageDisable() {
        storageEnabled = false;
        SendCustomEventDelayedFrames(nameof(EnableStorage), 50);
    }

    public void EnableStorage() {
        storageEnabled = true;
    }
}
