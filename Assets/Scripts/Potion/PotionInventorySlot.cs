
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
}
