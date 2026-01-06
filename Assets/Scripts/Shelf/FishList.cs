
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class FishList : UdonSharpBehaviour
{
    public FishListEntry[] fishListEntries;
    public FishLog fishLog;
    public FishLogSynced fishLogSynced;
    public Water[] lakes;
    public GameObject content;
    public GameObject fishListEntryPrefab;

    public void UpdateFishList() 
    {
        foreach (FishListEntry entry in fishListEntries)
        {
            entry.UpdateText();
        }
    }

    [ContextMenu("Populate Fish List")]
    public void PopulateFishList()
    {
        while (content.transform.childCount > 0)
        {
            DestroyImmediate(content.transform.GetChild(0).gameObject);
        }
        fishListEntries = new FishListEntry[0];
        foreach (Water lake in lakes) {
            foreach (FishData fishData in lake.common) {
                if (fishData.tags.Length > 0 && fishData.tags[0] == FishTag.Recipe) {
                    continue;
                }
                FishListEntry entry = GetFishListEntry(fishData);
                if (entry == null) {
                    GameObject newEntry = Instantiate(fishListEntryPrefab, content.transform);
                    newEntry.transform.localPosition = new Vector3(0, 0, 0);
                    newEntry.transform.localRotation = Quaternion.identity;
                    entry = newEntry.GetComponent<FishListEntry>();
                    newEntry.name = fishData.name;
                    entry.locationText.text = "Location: " + lake.name;
                    FishListEntry[] newFishListEntries = new FishListEntry[fishListEntries.Length + 1];
                    for (int i = 0; i < fishListEntries.Length; i++) {
                        newFishListEntries[i] = fishListEntries[i];
                    }
                    fishListEntries = newFishListEntries;
                    fishListEntries[fishListEntries.Length - 1] = entry;
                }
                else {
                    entry.locationText.text += ", " + lake.name;
                }
                entry.fishNameText.text = "???";
                entry.fishData = fishData;
                entry.fishLogEntry = fishLog.GetFishLogEntry(fishData);
                entry.fishLogEntrySynced = fishLogSynced.GetFishLogEntry(fishData);
            }
            foreach (FishData fishData in lake.uncommon) {
                if (fishData.tags.Length > 0 && fishData.tags[0] == FishTag.Recipe) {
                    continue;
                }
                FishListEntry entry = GetFishListEntry(fishData);
                if (entry == null) {
                    GameObject newEntry = Instantiate(fishListEntryPrefab, content.transform);
                    newEntry.transform.localPosition = new Vector3(0, 0, 0);
                    newEntry.transform.localRotation = Quaternion.identity;
                    entry = newEntry.GetComponent<FishListEntry>();
                    newEntry.name = fishData.name;
                    entry.locationText.text = "Location: " + lake.name;
                    FishListEntry[] newFishListEntries = new FishListEntry[fishListEntries.Length + 1];
                    for (int i = 0; i < fishListEntries.Length; i++) {
                        newFishListEntries[i] = fishListEntries[i];
                    }
                    fishListEntries = newFishListEntries;
                    fishListEntries[fishListEntries.Length - 1] = entry;
                }
                else {
                    entry.locationText.text += ", " + lake.name;
                }
                entry.fishNameText.text = "???";
                entry.fishData = fishData;
                entry.fishLogEntry = fishLog.GetFishLogEntry(fishData);
                entry.fishLogEntrySynced = fishLogSynced.GetFishLogEntry(fishData);
            }
            foreach (FishData fishData in lake.rare) {
                if (fishData.tags.Length > 0 && fishData.tags[0] == FishTag.Recipe) {
                    continue;
                }
                FishListEntry entry = GetFishListEntry(fishData);
                if (entry == null) {
                    GameObject newEntry = Instantiate(fishListEntryPrefab, content.transform);
                    newEntry.transform.localPosition = new Vector3(0, 0, 0);
                    newEntry.transform.localRotation = Quaternion.identity;
                    entry = newEntry.GetComponent<FishListEntry>();
                    newEntry.name = fishData.name;
                    entry.locationText.text = "Location: " + lake.name;
                    FishListEntry[] newFishListEntries = new FishListEntry[fishListEntries.Length + 1];
                    for (int i = 0; i < fishListEntries.Length; i++) {
                        newFishListEntries[i] = fishListEntries[i];
                    }
                    fishListEntries = newFishListEntries;
                    fishListEntries[fishListEntries.Length - 1] = entry;
                }
                else {
                    entry.locationText.text += ", " + lake.name;
                }
                entry.fishNameText.text = "???";
                entry.fishData = fishData;
                entry.fishLogEntry = fishLog.GetFishLogEntry(fishData);
                entry.fishLogEntrySynced = fishLogSynced.GetFishLogEntry(fishData);
            }
            foreach (FishData fishData in lake.epic) {
                if (fishData.tags.Length > 0 && fishData.tags[0] == FishTag.Recipe) {
                    continue;
                }
                FishListEntry entry = GetFishListEntry(fishData);
                if (entry == null) {
                    GameObject newEntry = Instantiate(fishListEntryPrefab, content.transform);
                    newEntry.transform.localPosition = new Vector3(0, 0, 0);
                    newEntry.transform.localRotation = Quaternion.identity;
                    entry = newEntry.GetComponent<FishListEntry>();
                    newEntry.name = fishData.name;
                    entry.locationText.text = "Location: " + lake.name;
                    FishListEntry[] newFishListEntries = new FishListEntry[fishListEntries.Length + 1];
                    for (int i = 0; i < fishListEntries.Length; i++) {
                        newFishListEntries[i] = fishListEntries[i];
                    }
                    fishListEntries = newFishListEntries;
                    fishListEntries[fishListEntries.Length - 1] = entry;
                }
                else {
                    entry.locationText.text += ", " + lake.name;
                }
                entry.fishNameText.text = "???";
                entry.fishData = fishData;
                entry.fishLogEntry = fishLog.GetFishLogEntry(fishData);
                entry.fishLogEntrySynced = fishLogSynced.GetFishLogEntry(fishData);
            }
            foreach (FishData fishData in lake.legendary) {
                if (fishData.tags.Length > 0 && fishData.tags[0] == FishTag.Recipe) {
                    continue;
                }
                FishListEntry entry = GetFishListEntry(fishData);
                if (entry == null) {
                    GameObject newEntry = Instantiate(fishListEntryPrefab, content.transform);
                    newEntry.transform.localPosition = new Vector3(0, 0, 0);
                    newEntry.transform.localRotation = Quaternion.identity;
                    entry = newEntry.GetComponent<FishListEntry>();
                    newEntry.name = fishData.name;
                    entry.locationText.text = "Location: " + lake.name;
                    FishListEntry[] newFishListEntries = new FishListEntry[fishListEntries.Length + 1];
                    for (int i = 0; i < fishListEntries.Length; i++) {
                        newFishListEntries[i] = fishListEntries[i];
                    }
                    fishListEntries = newFishListEntries;
                    fishListEntries[fishListEntries.Length - 1] = entry;
                }
                else {
                    entry.locationText.text += ", " + lake.name;
                }
                entry.fishNameText.text = "???";
                entry.fishData = fishData;
                entry.fishLogEntry = fishLog.GetFishLogEntry(fishData);
                entry.fishLogEntrySynced = fishLogSynced.GetFishLogEntry(fishData);
            }
        }
    }

    public FishListEntry GetFishListEntry(FishData fishData)
    {
        foreach (FishListEntry entry in fishListEntries)
        {
            if (entry.fishData == fishData)
            {
                return entry;
            }
        }
        return null;
    }
}
