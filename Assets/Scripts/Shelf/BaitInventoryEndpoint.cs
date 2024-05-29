
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BaitInventoryEndpoint : UdonSharpBehaviour
{
    public BaitButton[] buttons;
    public BaitInventory baitInventory;
    [UdonSynced] public bool endpointActive = false;

    public override void OnDeserialization()
    {
        if (!endpointActive) {
            DisableButtons();
        }
        else {
            EnableButtons();
        }
    }

    public void EnableEndPoint() {
        endpointActive = true;
        EnableButtons();
        RequestSerialization();
    }

    public void DisableEndPoint() {
        endpointActive = false;
        DisableButtons();
        RequestSerialization();
    }

    public void DisableButtons() {
        foreach (BaitButton button in buttons) {
            button.Disable();
        }
    }

    public void EnableButtons() {
        foreach (BaitButton button in buttons) {
            button.Enable();
        }
    }

    public int SpawnBait(Bait bait, Transform target) {
        return baitInventory.SpawnBait(bait, target);
    }

    public void UpdateButtonText(Bait bait, int nBait) {
        foreach (BaitButton button in buttons) {
            if (button.bait == bait) {
                button.UpdateCountText(nBait);
                if (endpointActive) button.Enable();
                return;
            }
        }
    }

}
