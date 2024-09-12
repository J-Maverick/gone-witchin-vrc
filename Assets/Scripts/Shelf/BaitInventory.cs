
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BaitInventory : UdonSharpBehaviour
{
    public BaitInventoryEndpoint[] endpoints;
    public BaitPool[] baitPools;

    public void Start() {
        foreach (BaitPool pool in baitPools) {
            if (pool.nBait >= 0) {
                UpdateButtons(pool.bait, pool.nBait);
            }
        }
        if (Networking.GetOwner(gameObject).isLocal) {
            foreach (BaitInventoryEndpoint endpoint in endpoints) {
                EnableEndpoint(endpoint);
            }
        }
    }

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

    public int SpawnBait(Bait bait, Transform target) {
        foreach (BaitPool pool in baitPools) {
            if (pool.bait == bait) {
                return pool.SpawnBait(target);
            }
        }
        return -1;
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
                UpdateButtons(bait, nBait);
            }
        }
    }

    public void EnableEndpoint(BaitInventoryEndpoint endpointToEnable) {
        // foreach (BaitInventoryEndpoint endpoint in endpoints) {
        //     endpoint.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DisableEndPoint");
        // }
        endpointToEnable.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "EnableEndPoint");
    }
}
