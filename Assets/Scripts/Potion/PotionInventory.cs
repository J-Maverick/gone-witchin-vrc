
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class PotionInventory : UdonSharpBehaviour
{
    public PotionInventorySlot[] slots;
    public GameObject itemEntryPrefab;
    public Transform inventoryUIParent;

    [ContextMenu("Clear Slots")]
    public void ClearSlots() {
        foreach (PotionInventorySlot slot in slots) {
            slot.buttons = new PotionInventoryButton[0];
        }
    }

    public PotionInventorySlot UpdateSlot(PotionInventoryButton button) {
        foreach (PotionInventorySlot slot in slots) {
            if (slot.ID == button.slotID) {
                // add button to buttons array
                PotionInventoryButton[] newButtons = new PotionInventoryButton[slot.buttons.Length + 1];
                for (int i = 0; i < slot.buttons.Length; i++) {
                    newButtons[i] = slot.buttons[i];
                }
                newButtons[slot.buttons.Length] = button;
                slot.buttons = newButtons;
                return slot;
            }
        }
        return null;
    }

    public void UpdateInventoryUI()
    {
        var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
        for (int i = 0; i < objects.Length; i++)
        {
            if (!Utilities.IsValid(objects[i])) continue;
            PotionInventoryReference inventoryRef = objects[i].GetComponentInChildren<PotionInventoryReference>();
            if (Utilities.IsValid(inventoryRef))
            {
                foreach (Transform child in inventoryRef.sharedInventoryUIParent)
                {
                    Destroy(child.gameObject);
                }
                int nPotions = 0;
                foreach (PotionInventorySlot slot in slots)
                {
                    if (slot.isHoldingItem)
                    {
                        PotionPool pool = slot.potionOcean.GetPoolByID(slot.liquidID);
                        if (pool != null)
                        {
                            GameObject itemEntry = Instantiate(itemEntryPrefab, inventoryRef.sharedInventoryUIParent);
                            itemEntry.transform.localPosition = Vector3.zero;
                            PotionInventoryEntry entry = itemEntry.GetComponent<PotionInventoryEntry>();
                            entry.SetItem(pool.liquid.name, slot.ID);
                        }
                        else
                        {
                            Debug.LogFormat("{0}: Unknown Item", name);
                        }
                        nPotions++;
                    }
                }

                inventoryRef.sharedInventoryText.text = "Shared (" + nPotions + "/16)";
            }
        }
        Debug.LogFormat("{0}: UpdateInventoryUI", name);
    }

    public void DelayedInventoryUIUpdate()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateInventoryUI));
    }

    public void AddToInventory(Bottle bottle)
    {
        Debug.LogFormat("{0}: AddToInventory: {1}", name, bottle.name);
        foreach (PotionInventorySlot slot in slots)
        {
            bool addedToSlot = slot.SendBottle(bottle);
            if (addedToSlot)
            {
                // Update the button state
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateInventoryUI));
                SendCustomEventDelayedSeconds(nameof(DelayedInventoryUIUpdate), 1f);
                break;
            }

        }
    }

    public void SpawnItem(int slotID) {
        Debug.LogFormat("{0}: SpawnItem: {1}", name, slotID);
        foreach (PotionInventorySlot slot in slots) {
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
        SendCustomEventDelayedSeconds(nameof(DelayedInventoryUIUpdate), 1f);
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        UpdateInventoryUI();
    }

}
