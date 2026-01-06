
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PotionInventorySlot : UdonSharpBehaviour
{
    public int ID;
    [UdonSynced] public int liquidID = -1;
    [UdonSynced] public int bottleID = -1;
    [UdonSynced] public bool isHoldingItem = false;
    [UdonSynced] public bool buttonActive = false;
    public PotionOcean potionOcean;
    public PotionInventoryButton[] buttons;


    public override void OnPreSerialization()
    {
        Debug.LogFormat("{0}: OnPreSerialization", name);
        foreach (PotionInventoryButton button in buttons) {
            button.UpdateButtonState();
            button.UpdateTextState();
        }
    }

    public override void OnDeserialization()
    {
        Debug.LogFormat("{0}: OnDeserialization", name);
        foreach (PotionInventoryButton button in buttons) {
            button.UpdateButtonState();
            button.UpdateTextState();
        }
    }

    public void Serialize() {
        Debug.LogFormat("{0}: Serialize", name);
        RequestSerialization();
        foreach (PotionInventoryButton button in buttons) {
            button.UpdateButtonState();
            button.UpdateTextState();
        }
    }

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
                buttonActive = false;
                spawnedPotion.transform.SetPositionAndRotation(target.position, target.rotation);
            }
        }
        Serialize();
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
            BottleSync sync = bottle.GetComponentInChildren<BottleSync>();
            liquidID = bottle.liquid.ID;
            bottleID = sync.BottleID;
            isHoldingItem = true;
            buttonActive = true;
            BottleCollision bottleCollision = bottle.GetComponent<BottleCollision>();
            bottleCollision.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(bottleCollision.Despawn));
        }
        Serialize();
        return true;
    }
}
