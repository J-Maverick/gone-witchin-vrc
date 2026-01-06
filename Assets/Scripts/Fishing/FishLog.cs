
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using VRC.SDK3.Persistence;

[ExecuteInEditMode]
public class FishLog : UdonSharpBehaviour
{
    public FishLogEntry[] fishLogEntries;
    public FishLogSynced fishLogSynced;
    public TMP_Text text;

    public FishLogEntry GetFishLogEntry(FishData fishData)
    {
        foreach (FishLogEntry entry in fishLogEntries)
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
        FishLogEntry entry = GetFishLogEntry(fishData);
        if (entry == null)
        {
            Debug.LogError("FishLogEntry not found for fishData: " + fishData);
            return;
        }
        if (weight > entry.largestCaught)
        {
            entry.largestCaught = weight;
            PlayerData.SetFloat(fishData.name + DataKeys.LargestCaught, weight);

        }
        if (weight < entry.smallestCaught || entry.smallestCaught < 0f)
        {
            entry.smallestCaught = weight;
            PlayerData.SetFloat(fishData.name + DataKeys.SmallestCaught, weight);
        }
        fishLogSynced.AddFish(fishData, weight);
    }

    [ContextMenu("Update Text")]
    public void UpdateText() {
        string logText = "";
        foreach (FishLogEntry entry in fishLogEntries)
        {
            logText += entry.fishData.name + ": " + entry.largestCaught + "\n";
        }
        text.text = logText;
    }

}
