using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FishLogPopulator : MonoBehaviour
{
    public FishLog fishLog;
    public FishLogSynced fishLogSynced;
    public FishDataPool fishDataPool;
    [ContextMenu("Create Fish Log Entries")]
    public void CreateFishLogEntries()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        fishLog.fishLogEntries = new FishLogEntry[fishDataPool.fishData.Length];
        int i = 0;
        foreach (FishData fishData in fishDataPool.fishData)
        {
            GameObject entry = new GameObject(fishData.name);
            entry.transform.parent = transform;
            FishLogEntry logEntry = entry.AddComponent<FishLogEntry>();
            logEntry.fishData = fishData;
            fishLog.fishLogEntries[i] = logEntry;
            i++;
        }
    }

    [ContextMenu("Create Fish Log Synced Entries")]
    public void CreateFishLogSyncedEntries()
    {
        foreach (Transform child in fishLogSynced.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        fishLogSynced.fishLogEntries = new FishLogEntrySynced[fishDataPool.fishData.Length];
        int i = 0;
        foreach (FishData fishData in fishDataPool.fishData)
        {
            GameObject entry = new GameObject(fishData.name);
            entry.transform.parent = fishLogSynced.transform;
            FishLogEntrySynced logEntry = entry.AddComponent<FishLogEntrySynced>();
            logEntry.fishData = fishData;
            logEntry.fishLog = fishLogSynced;
            fishLogSynced.fishLogEntries[i] = logEntry;
            i++;
        }
    }
}
