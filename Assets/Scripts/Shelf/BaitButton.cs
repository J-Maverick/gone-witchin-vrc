
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.SDK3.Persistence;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BaitButton : UdonSharpBehaviour
{
    public Bait bait;
    public Transform spawnTarget;
    public Text baitCountText;
    [UdonSynced] public string countText;
    public BaitInventoryEndpoint baitInventoryEndpoint;
    public Collider buttonCollider;

    public override void Interact()
    {
        Interaction();
    }

    public override void OnDeserialization()
    {
        RefreshCountText();
    }

    public void Interaction() {
        baitInventoryEndpoint.SpawnBait(bait, spawnTarget);
    }

    public void Enable() {
        if (countText != "") {
            buttonCollider.enabled = true;
        }
        else {
            buttonCollider.enabled = false;
        }
    }

    public void Disable() {
        buttonCollider.enabled = false;
    }

    public void UpdateCountText(int newCount) {
        if (newCount < 0 && PlayerData.GetInt(Networking.LocalPlayer, bait.name + DataKeys.BaitCount) == 0) {
            countText = "";
        }
        else {
            countText = string.Format("{0}\n[{1}]", newCount, PlayerData.GetInt(Networking.LocalPlayer, bait.name + DataKeys.BaitCount));
        }
        baitCountText.text = countText;
        RequestSerialization();
    }

    public void RefreshCountText() {
        baitCountText.text = countText;
    }
}
