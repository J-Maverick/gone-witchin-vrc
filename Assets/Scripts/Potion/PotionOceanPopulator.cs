using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Components;

[ExecuteInEditMode]
public class PotionOceanPopulator : MonoBehaviour
{
    public GameObject potionPoolPrefab;
    public GameObject[] potionPrefabs;
    public int nPotionsPerPool = 15;

    [ContextMenu("Populate Ocean")]
    public void PopulateOcean() {
        PotionOcean potionOcean = GetComponent<PotionOcean>();

        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        PotionPool[] potionPools = new PotionPool[potionPrefabs.Length];
        int poolIndex = 0;
        foreach (GameObject potionPrefab in potionPrefabs) {
            // Make a new pool for the potion type
            GameObject pool = GameObject.Instantiate(potionPoolPrefab);
            pool.transform.SetParent(transform);
            PotionPool potionPool = pool.GetComponent<PotionPool>();
            VRCObjectPool objectPool = pool.GetComponent<VRCObjectPool>();

            // Give new pool a name
            GameObject tempPotion = GameObject.Instantiate(potionPrefab);
            LiquidMaterial liquid = tempPotion.GetComponent<Bottle>().liquid;
            pool.name = liquid.name + " Pool";
            potionPool.liquid = liquid;
            GameObject.DestroyImmediate(tempPotion);

            // Add potions to the pool
            GameObject[] objectPoolArray = new GameObject[nPotionsPerPool];
            for (int i = 0; i < nPotionsPerPool; i++) {
                GameObject newPotion = GameObject.Instantiate(potionPrefab);
                newPotion.transform.SetParent(pool.transform);
                newPotion.SetActive(false);

                objectPoolArray[i] = newPotion;
            }
            objectPool.Pool = objectPoolArray;
            potionPools[poolIndex] = potionPool;
            poolIndex++;
        }
        potionOcean.potionPools = potionPools;
    }

}
