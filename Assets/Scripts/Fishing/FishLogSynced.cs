
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using System;
// using System.Text.RegularExpressions;

[ExecuteInEditMode]
public class FishLogSynced : UdonSharpBehaviour
{
    public FishLogEntrySynced[] fishLogEntries;
    public TMP_Text text;
    public FishLogEntrySynced GetFishLogEntry(FishData fishData)
    {
        foreach (FishLogEntrySynced entry in fishLogEntries)
        {
            if (entry.fishData == fishData)
            {
                return entry;
            }
        }
        return null;
    }

    public void AddFish(FishData fishData, float weight)
    {
        FishLogEntrySynced entry = GetFishLogEntry(fishData);
        if (entry == null)
        {
            Debug.LogError("FishLogEntry not found for fishData: " + fishData);
            return;
        }
        entry.UpdateStats(weight);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateText));
    }

    [ContextMenu("Update Text")]
    public void UpdateText() {
        // string logText = "";
        // foreach (FishLogEntrySynced entry in fishLogEntries)
        // {
        //     logText += entry.fishData.name + ": " + entry.largestCaught + " [" + entry.playerName + "]" + "\n";
        // }
        // text.text = logText;
        var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
        for (int i = 0; i < objects.Length; i++)
        {
            if (!Utilities.IsValid(objects[i])) continue;
            PotionInventoryReference inventoryRef = objects[i].GetComponentInChildren<PotionInventoryReference>();
            if (Utilities.IsValid(inventoryRef)) {
                inventoryRef.fishList.UpdateFishList();
            }
        }
    }
}
