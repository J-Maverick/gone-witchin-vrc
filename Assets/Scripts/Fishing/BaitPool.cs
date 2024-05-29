
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BaitPool : UdonSharpBehaviour
{
    public VRCObjectPool pool;
    [UdonSynced] public int nBait = -1;
    public Bait bait;

    public int SpawnBait(Transform target) {
        if (nBait > 0) {
            GameObject spawnedObject = pool.TryToSpawn();
            if (spawnedObject != null) {
                Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                spawnedObject.transform.position = target.position;
                nBait -= 1;
                if (nBait < 0) nBait = 0;
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
