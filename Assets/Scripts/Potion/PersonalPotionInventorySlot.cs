
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class PersonalPotionInventorySlot : UdonSharpBehaviour
{

    public int ID;
    public int liquidID = -1;
    public int bottleID = -1;
    public bool isHoldingItem = false;
    public PotionOcean potionOcean;

    public void SpawnItem(Transform target) {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (isHoldingItem) {
            GameObject spawnedPotion = potionOcean.TryToSpawnByID(liquidID);
            if (spawnedPotion != null) {
                Networking.SetOwner(Networking.LocalPlayer, spawnedPotion);
                Debug.LogFormat("{0}: Spawned {1}", name, spawnedPotion.name);
                BottleSync sync = spawnedPotion.GetComponentInChildren<BottleSync>();
                Networking.SetOwner(Networking.LocalPlayer, sync.gameObject);
                sync.SetBottleType(bottleID);
                liquidID = -1;
                bottleID = -1;
                isHoldingItem = false;
                PlayerData.SetInt("PotionInventorySlot" + ID + "_Liquid", -1);
                PlayerData.SetInt("PotionInventorySlot" + ID + "_Bottle", -1);
                spawnedPotion.transform.SetPositionAndRotation(target.position, target.rotation);
            }
        }
        RequestSerialization();
    }

    public bool SendBottle(Bottle bottle) {
        Debug.LogFormat("{0}: SendBottle: {1}", name, bottle.name);
        if (isHoldingItem) {
            return false;
        }
        ReagentBottle reagentBottle = bottle.GetComponent<ReagentBottle>();
        if (reagentBottle != null) {
            return false;
        }
        if (!Networking.GetOwner(bottle.gameObject).isLocal) {
            return false;
        }
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        PotionPool potionPool = potionOcean.GetPoolByID(bottle.liquid.ID);
        if (potionPool != null) {
            Networking.SetOwner(Networking.GetOwner(bottle.gameObject), gameObject);
            Networking.SetOwner(Networking.GetOwner(bottle.gameObject), gameObject);
            BottleSync sync = bottle.GetComponentInChildren<BottleSync>();
            liquidID = bottle.liquid.ID;
            bottleID = sync.BottleID;
            isHoldingItem = true;
            
            PlayerData.SetInt("PotionInventorySlot" + ID + "_Liquid", liquidID);
            PlayerData.SetInt("PotionInventorySlot" + ID + "_Bottle", bottleID);
            BottleCollision bottleCollision = bottle.GetComponent<BottleCollision>();
            bottleCollision.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(bottleCollision.Despawn));
        }
        RequestSerialization();
        return true;
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        Debug.LogFormat("{0}: OnPlayerRestored", name);
        if (player.isLocal) {
            liquidID = PlayerData.GetInt(Networking.LocalPlayer, "PotionInventorySlot" + ID + "_Liquid");
            bottleID = PlayerData.GetInt(Networking.LocalPlayer, "PotionInventorySlot" + ID + "_Bottle");
            if (liquidID == 0) {
                liquidID = -1;
                bottleID = -1;
                PlayerData.SetInt("PotionInventorySlot" + ID + "_Liquid", -1);
                PlayerData.SetInt("PotionInventorySlot" + ID + "_Bottle", -1);
            }
            if (potionOcean.GetPoolByID(liquidID) == null) {
                liquidID = -1;
                bottleID = -1;
                PlayerData.SetInt("PotionInventorySlot" + ID + "_Liquid", -1);
                PlayerData.SetInt("PotionInventorySlot" + ID + "_Bottle", -1);
            }
            if (liquidID != -1 && bottleID != -1)
            {
                isHoldingItem = true;
            }
            else
            {
                isHoldingItem = false;
            }
        }
    }

}
