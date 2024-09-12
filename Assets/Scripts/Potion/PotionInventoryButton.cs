
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PotionInventoryButton : UdonSharpBehaviour
{
    public int slotID;
    public TMP_Text text;
    public Animator animator;
    public PotionInventorySnap potionInventorySnap;
    public PotionInventorySlot slot;
    public PotionInventory inventory;
    [UdonSynced] public bool triggerActive = true;

    [ContextMenu("Add Slot to Inventory")]
    public void AddSlotToInventory() {
        slot = inventory.UpdateSlot(this);
    }

    public void UpdateButtonState()
    {
        Debug.LogFormat("{0}: UpdateButtonState", name);
        animator.SetBool("ButtonActive", slot.buttonActive);
    }

    public void UpdateTextState() {
        Debug.LogFormat("{0}: UpdateTextState", name);
        if (slot.isHoldingItem) {
            PotionPool pool = slot.potionOcean.GetPoolByID(slot.liquidID);
            if (pool != null) {
                text.text = pool.liquid.name;
            }
            else {
                text.text = "Unknown Item";
            }
        }
        else {
            text.text = "No Item Stored";
            SendCustomEventDelayedFrames(nameof(SetTriggerActive), 2);
        }
    }
    public override void Interact()
    {
        if (slot.isHoldingItem) {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, slot.gameObject);
            SpawnItem(); 
        }
    }

    public void SpawnItem() {
        if (slot.isHoldingItem) {
            GameObject spawnedPotion = slot.potionOcean.TryToSpawnByID(slot.liquidID);
            if (spawnedPotion != null) {
                Networking.SetOwner(Networking.LocalPlayer, spawnedPotion);
                Debug.LogFormat("{0}: Spawned {1}", name, spawnedPotion.name);
                BottleSync sync = spawnedPotion.GetComponentInChildren<BottleSync>();
                Networking.SetOwner(Networking.LocalPlayer, sync.gameObject);
                sync.SetBottleType(slot.bottleID);
                slot.liquidID = -1;
                slot.bottleID = -1;
                slot.isHoldingItem = false;
                triggerActive = false;
                slot.buttonActive = false;
                spawnedPotion.transform.SetPositionAndRotation(potionInventorySnap.transform.position, potionInventorySnap.transform.rotation);
            }
        }
        RequestSerialization();
        slot.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(slot.Serialize));
    }

    public void SendBottle(Bottle bottle) {
        Debug.LogFormat("{0}: SendBottle: {1}", name, bottle.name);
        if (slot.isHoldingItem || !triggerActive) {
            return;
        }
        ReagentBottle reagentBottle = bottle.GetComponent<ReagentBottle>();
        if (reagentBottle != null) {
            return;
        }
        if (!Networking.GetOwner(bottle.gameObject).isLocal) {
            return;
        }
        PotionPool potionPool = slot.potionOcean.GetPoolByID(bottle.liquid.ID);
        if (potionPool != null) {
            Networking.SetOwner(Networking.GetOwner(bottle.gameObject), gameObject);
            Networking.SetOwner(Networking.GetOwner(bottle.gameObject), slot.gameObject);
            BottleSync sync = bottle.GetComponentInChildren<BottleSync>();
            slot.liquidID = bottle.liquid.ID;
            slot.bottleID = sync.BottleID;
            slot.isHoldingItem = true;
            triggerActive = false;
            slot.buttonActive = true;
            BottleCollision bottleCollision = bottle.GetComponent<BottleCollision>();
            bottleCollision.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(bottleCollision.Despawn));
        }
        RequestSerialization();
        slot.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(slot.Serialize));
    }

    public void ClearBottle(Bottle bottle) {
        Debug.LogFormat("{0}: ClearBottle: {1}", name, bottle.name);
        if (slot.isHoldingItem) {
            return;
        }
        if (!Networking.GetOwner(bottle.gameObject).isLocal) {
            return;
        }
        PotionPool potionPool = slot.potionOcean.GetPoolByID(bottle.liquid.ID);
        if (potionPool != null) {
            Networking.SetOwner(Networking.GetOwner(bottle.gameObject), gameObject);
            triggerActive = true;
        }
        RequestSerialization();
        slot.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(slot.Serialize));
    }

    public void SetTriggerActive() {
        triggerActive = true;
        RequestSerialization();
    }
}
