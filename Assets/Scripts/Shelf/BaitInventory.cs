
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;
using VRC.SDK3.Components;

[ExecuteInEditMode]
public class BaitInventory : UdonSharpBehaviour
{
    public BaitInventoryEndpoint[] endpoints;
    public BaitPool[] baitPools;
    public Bait[] baits;
    public int nBaitsPerPool = 25;
    public GameObject baitPoolPrefab;
    public GameObject baitPrefab;
    
    public void Start() {
        // UpdateAllButtons();
        // if (Networking.GetOwner(gameObject).isLocal) {
        //     foreach (BaitInventoryEndpoint endpoint in endpoints) {
        //         EnableEndpoint(endpoint);
        //     }
        // }
    }

    public void AddEndpoint(BaitInventoryEndpoint newEndpoint) {
        BaitInventoryEndpoint[] newEndpoints = new BaitInventoryEndpoint[endpoints.Length + 1];
        for (int i = 0; i < endpoints.Length; i++) {
            newEndpoints[i] = endpoints[i];
        }
        newEndpoints[endpoints.Length] = newEndpoint;
        endpoints = newEndpoints;
        Start();
    }

    public void RemoveEndpoint(BaitInventoryEndpoint endpointToRemove) {
        BaitInventoryEndpoint[] newEndpoints = new BaitInventoryEndpoint[endpoints.Length - 1];
        int j = 0;
        for (int i = 0; i < endpoints.Length; i++) {
            if (endpoints[i] != endpointToRemove) {
                newEndpoints[j] = endpoints[i];
                j++;
            }
        }
        endpoints = newEndpoints;
        Start();
    }

    // public override void OnPlayerLeft(VRCPlayerApi player)
    // {    
    //     var objects = Networking.GetPlayerObjects(player);
    //     for (int i = 0; i < objects.Length; i++)
    //     {
    //         if (!Utilities.IsValid(objects[i])) continue;
    //         PersonalInventory personalInventory = objects[i].GetComponentInChildren<PersonalInventory>();
    //         if (Utilities.IsValid(personalInventory)) {
    //             RemoveEndpoint(personalInventory.baitInventory);
    //         }
    //     }
    // }


    public Bait GetBaitByIndex(int index) {
        if (index < 0) {
            return null;
        }
        return baitPools[index].bait;
    }

    public int GetBaitIndex(Bait bait) {
        if (bait == null) {
            return -1;
        }
        for (int i = 0; i < baitPools.Length; i++) {
            if (baitPools[i].bait == bait) {
                return i;
            }
        }
        return -1;
    }

    public BaitPool GetBaitPool(Bait bait) {
        if (bait == null) {
            return null;
        }
        for (int i = 0; i < baitPools.Length; i++) {
            if (baitPools[i].bait == bait) {
                return baitPools[i];
            }
        }
        return null;
    }

    public void SpawnBait(Bait bait, Transform target) {
        foreach (BaitPool pool in baitPools) {
            if (pool.bait == bait) {
                Networking.SetOwner(Networking.LocalPlayer, pool.gameObject);
                pool.SpawnBait(target);
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateBaitUI));
            }
        }
    }

    public int BaitInventoryCount(Bait bait) {
        foreach (BaitPool pool in baitPools) {
            if (pool.bait == bait) {
                return pool.nBait;
            }
        }
        return -1;
    }

    public void UpdateBaitUI() {
        var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
        for (int i = 0; i < objects.Length; i++)
        {
            if (!Utilities.IsValid(objects[i])) continue;
            PotionInventoryReference inventoryRef = objects[i].GetComponentInChildren<PotionInventoryReference>();
            if (Utilities.IsValid(inventoryRef)) {
                inventoryRef.baitInventoryUI.UpdateBaitText();
            }
        }
    }

    public void UpdateAllButtons() {
        foreach (BaitPool pool in baitPools) {
            if (pool.nBait >= 0) {
                UpdateButtons(pool.bait, pool.nBait);
            }
        }
    }

    public void UpdateButtons(Bait bait, int nBait) {
        foreach (BaitInventoryEndpoint endpoint in endpoints) {
            endpoint.UpdateButtonText(bait, nBait);
        }
    }

    public void AddBait(Bait bait) {
        foreach (BaitPool pool in baitPools) {
            if (pool.bait == bait) {
                int nBait = pool.AddBait();
                // UpdateButtons(bait, nBait);
            }
        }
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateBaitUI));
    }

    public void EnableEndpoint(BaitInventoryEndpoint endpointToEnable) {
        // foreach (BaitInventoryEndpoint endpoint in endpoints) {
        //     endpoint.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DisableEndPoint");
        // }
        endpointToEnable.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(BaitInventoryEndpoint.EnableEndPoint));
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (player.isLocal && player.isMaster) {
            foreach (BaitPool pool in baitPools) {
                int nBait = 0;
                bool hasBait = PlayerData.TryGetInt(player, pool.bait.name + DataKeys.BaitCount, out nBait);
                if (hasBait) {
                    pool.nBait = nBait;
                    // UpdateButtons(pool.bait, nBait);
                }
            }
            UpdateBaitUI();
        }
    }

    [ContextMenu("Populate Bait Pools")]
    public void PopulateBaitPools() {
        BaitInventory baitInventory = GetComponent<BaitInventory>();

        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        baitPools = new BaitPool[baits.Length];
        int poolIndex = 0;
        foreach (Bait bait in baits) {
            // Make a new pool for the potion type
            GameObject pool = GameObject.Instantiate(baitPoolPrefab);
            pool.transform.SetParent(transform);
            BaitPool baitPool = pool.GetComponent<BaitPool>();
            VRCObjectPool objectPool = pool.GetComponent<VRCObjectPool>();

            // Give new pool a name
            pool.name = bait.name + " Pool";
            baitPool.bait = bait;

            // Add potions to the pool
            GameObject[] objectPoolArray = new GameObject[nBaitsPerPool];
            for (int i = 0; i < nBaitsPerPool; i++) {
                GameObject newBait = GameObject.Instantiate(baitPrefab);
                newBait.transform.SetParent(pool.transform);
                newBait.SetActive(false);
                BaitItem baitItem = newBait.GetComponent<BaitItem>();
                baitItem.bait = bait;
                baitItem.baitPool = objectPool;
                MeshFilter meshFilter = newBait.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = newBait.GetComponent<MeshRenderer>();
                MeshCollider meshCollider = newBait.GetComponent<MeshCollider>();
                meshCollider.sharedMesh = bait.mesh;
                meshFilter.mesh = bait.mesh;
                meshRenderer.material = bait.material;
                VRCPickup pickup = newBait.GetComponent<VRCPickup>();
                pickup.InteractionText = bait.name;
                pickup.UseText = bait.name;
                objectPoolArray[i] = newBait;
            }
            objectPool.Pool = objectPoolArray;
            baitPools[poolIndex] = baitPool;
            poolIndex++;
        }
    }
}
