
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class PotionInventory : UdonSharpBehaviour
{
    public PotionInventorySlot[] slots;

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
}
