
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PersonalPotionInventory : UdonSharpBehaviour
{

    public PersonalPotionInventorySlot[] slots;
    public GameObject itemEntryPrefab;
    public Transform inventoryUIParent;


    public void UpdateInventoryUI() {
        var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
        for (int i = 0; i < objects.Length; i++)
        {
            if (!Utilities.IsValid(objects[i])) continue;
            PotionInventoryReference inventoryRef = objects[i].GetComponentInChildren<PotionInventoryReference>();
            if (Utilities.IsValid(inventoryRef)) {
                foreach (Transform child in inventoryRef.personalInventoryUIParent) {
                    Destroy(child.gameObject);
                }
                int nPotions = 0;
                foreach (PersonalPotionInventorySlot slot in slots) {
                    if (slot.isHoldingItem) {
                        PotionPool pool = slot.potionOcean.GetPoolByID(slot.liquidID);
                        if (pool != null) {
                            GameObject itemEntry = Instantiate(itemEntryPrefab, inventoryRef.personalInventoryUIParent);
                            itemEntry.transform.localPosition = Vector3.zero;
                            PersonalPotionInventoryEntry entry = itemEntry.GetComponent<PersonalPotionInventoryEntry>();
                            entry.SetItem(pool.liquid.name, slot.ID);
                        }
                        else {
                            Debug.LogFormat("{0}: Unknown Item", name);
                        }
                        nPotions++;
                    }
                }
                inventoryRef.personalInventoryText.text = "Personal (" + nPotions + "/16)";
            }
        }
    }

    public void AddToInventory(Bottle bottle) {
        foreach (PersonalPotionInventorySlot slot in slots) {
            bool addedToSlot = slot.SendBottle(bottle);
            if (addedToSlot) {
                // Update the button state
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateInventoryUI));
                break;
            }

        }
    }

    public void SpawnItem(int slotID) {
        Debug.LogFormat("{0}: SpawnItem: {1}", name, slotID);
        foreach (PersonalPotionInventorySlot slot in slots) {
            if (slot.ID == slotID) {
                var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
                for (int i = 0; i < objects.Length; i++)
                {
                    if (!Utilities.IsValid(objects[i])) continue;
                    PotionInventoryReference inventoryRef = objects[i].GetComponentInChildren<PotionInventoryReference>();
                    if (Utilities.IsValid(inventoryRef)) {
                        inventoryRef.potionInventoryStorage.TempStorageDisable();
                        slot.SpawnItem(inventoryRef.spawnTarget);
                    }
                }
                break;
            }
        }
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateInventoryUI));
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        SendCustomEventDelayedFrames(nameof(UpdateInventoryUI), 2);
    }

}
