
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using VRC.SDK3.Persistence;

public class BaitInventoryEntry : UdonSharpBehaviour
{
    public BaitInventory baitInventory;
    public BaitInventoryUI baitInventoryUI;
    public Bait bait;
    public TMP_Text nameText;
    public TMP_Text countText;

    public void UpdateText() {
        // Debug.LogFormat("{0}: BaitInventoryEntry.UpdateText() - Updating text for {1}", this, bait.name); 
        BaitPool baitPool = baitInventory.GetBaitPool(bait);
        if (baitPool == null) {
            Debug.LogFormat("{0}: BaitInventoryEntry.UpdateText() - BaitPool not found for {1}", this, bait.name);
            return;
        }
        int sharedCount = baitPool.nBait;
        int personalCount;
        bool personalCountFound = PlayerData.TryGetInt(Networking.LocalPlayer, bait.name + DataKeys.BaitCount, out personalCount);
        countText.text = string.Format(@"Shared: {0}
Personal: {1}", sharedCount < 0 ? 0 : sharedCount, personalCountFound ? personalCount : 0);
        if (sharedCount < 0 && !personalCountFound) {
            nameText.text = "???";
        }
        else {
            nameText.text = bait.name;
        }
    }

    public void SpawnBait() {
        baitInventory.SpawnBait(bait, baitInventoryUI.spawnTarget);
    }
}
