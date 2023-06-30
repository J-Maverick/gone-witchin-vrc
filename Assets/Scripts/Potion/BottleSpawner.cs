
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
            spawnedObject.transform.SetPositionAndRotation(spawnTarget.position, spawnTarget.rotation);
        }
        return spawnedObject;
    }

    public void Despawn(GameObject objectToDespawn) {
        pool.Return(objectToDespawn);
    }
}
