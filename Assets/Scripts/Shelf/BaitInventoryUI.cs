
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class BaitInventoryUI : UdonSharpBehaviour
{
    public Transform content;
    public GameObject baitPrefab;
    public BaitInventory baitInventory;
    public BaitInventoryEntry[] baitEntries;
    public Transform spawnTarget;

    [ContextMenu("Instantiate Bait")]
    public void InstantiateBait() 
    {
        //delete all children immediately
        while (content.childCount > 0)
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }

        foreach (BaitPool baitPool in baitInventory.baitPools)
        {
            GameObject baitObject = Instantiate(baitPrefab, content);
            baitObject.name = baitPool.bait.name;
            baitObject.transform.localPosition = Vector3.zero;
            baitObject.transform.localRotation = Quaternion.identity;
            BaitInventoryEntry entry = baitObject.GetComponent<BaitInventoryEntry>();
            entry.bait = baitPool.bait;
            entry.baitInventory = baitInventory;
            entry.nameText.text = "???";
            entry.baitInventoryUI = this;
            BaitInventoryEntry[] newEntries = new BaitInventoryEntry[baitEntries.Length + 1];
            for (int i = 0; i < baitEntries.Length; i++)
            {
                newEntries[i] = baitEntries[i];
            }
            newEntries[baitEntries.Length] = entry;
            baitEntries = newEntries;
        }
    }

    public void UpdateBaitText() {
        foreach (BaitInventoryEntry entry in baitEntries) {
            entry.UpdateText();
        }
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) {
            return;
        }
        UpdateBaitText();
    }

}
