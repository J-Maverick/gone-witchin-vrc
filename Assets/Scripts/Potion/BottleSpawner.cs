
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class BottleSpawner : UdonSharpBehaviour
{
    public VRCObjectPool pool;
    public Transform spawnTarget;

    public GameObject Spawn() {
        GameObject spawnedObject = pool.TryToSpawn();
        if (spawnedObject != null) {
            Networking.SetOwner(Networking.LocalPlayer, spawnedObject);
            spawnedObject.transform.SetPositionAndRotation(spawnTarget.position, spawnTarget.rotation);
            BottleSync sync = spawnedObject.GetComponentInChildren<BottleSync>();
            if (sync != null) {
                Networking.SetOwner(Networking.LocalPlayer, sync.gameObject);
            }
        }
        return spawnedObject;
    }

    public void Despawn(GameObject objectToDespawn) {
        if (Networking.GetOwner(pool.gameObject).isLocal) {
            pool.Return(objectToDespawn);
        }
    }
}
