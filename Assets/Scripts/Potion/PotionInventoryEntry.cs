
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class PotionInventoryEntry : UdonSharpBehaviour
{
    public int slotID = 0;
    public TMP_Text text;
    public PotionInventory potionInventory;

    public void SetItem(string itemName, int slotNumber)
    {
        slotID = slotNumber;
        text.text = itemName;
    }

    public void SpawnItem() {
        Debug.LogFormat("{0}: SpawnItem", name);
        potionInventory.SpawnItem(slotID);
    }
}
