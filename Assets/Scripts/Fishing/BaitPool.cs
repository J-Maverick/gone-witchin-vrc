
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BaitPool : UdonSharpBehaviour
{
    public VRCObjectPool pool;
    [UdonSynced] public int nBait = -1;
    public Bait bait;

    public int SpawnBait(Transform target) {
        if (nBait > 0 || PlayerData.GetInt(Networking.LocalPlayer, bait.name + DataKeys.BaitCount) > 0) {
            GameObject spawnedObject = pool.TryToSpawn();
            if (spawnedObject != null) {
                Networking.SetOwner(Networking.LocalPlayer, spawnedObject);
                Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                spawnedObject.transform.position = target.position;
                nBait -= 1;
                if (nBait < 0) nBait = 0;
                if (PlayerData.GetInt(Networking.LocalPlayer, bait.name + DataKeys.BaitCount) > 0) {
                    PlayerData.SetInt(bait.name + DataKeys.BaitCount, PlayerData.GetInt(Networking.LocalPlayer, bait.name + DataKeys.BaitCount) - 1);
                }
            }
        }
        RequestSerialization();
        return nBait;
    }

    public int AddBait() {
        if (nBait < 0) nBait = 0;
        nBait += 1;
        RequestSerialization();
        return nBait;
    }
}
