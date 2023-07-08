using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Components;

[ExecuteInEditMode]
public class BottlePoolHandler : MonoBehaviour
{
    public GameObject bottlePrefab;
    public int nBottles = 10;

    public VRCObjectPool objectPool;
    public BottleSpawner spawner;
    public Cauldron cauldron;
    public LiquidList recipeList;

    [ContextMenu("Update Bottles")]
    public void UpdateBottles() {
        while (objectPool.transform.childCount > 0) {
            DestroyImmediate(objectPool.transform.GetChild(0).gameObject);
        }

        name = string.Format("Spawner_{0}", bottlePrefab.name);
    
        for (int i = 0; i < nBottles; i++) {
            GameObject bottle = GameObject.Instantiate(bottlePrefab);
            bottle.transform.SetParent(objectPool.transform);
            bottle.transform.SetPositionAndRotation(objectPool.transform.position, objectPool.transform.rotation);
            bottle.GetComponent<ReagentBottle>().spawner = spawner;
            bottle.GetComponentInChildren<LiquidContact>().cauldron = cauldron;
            bottle.GetComponentInChildren<ReagentBottleSync>().liquidList = recipeList;
            bottle.SetActive(false);
        }
        objectPool.Pool = new GameObject[0];
    }
}
