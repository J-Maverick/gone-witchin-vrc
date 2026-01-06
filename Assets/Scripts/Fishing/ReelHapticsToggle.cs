
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class ReelHapticsToggle : UdonSharpBehaviour
{
    public Toggle toggle;

    public void ToggleHaptics()
    {
        var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
        for (int i = 0; i < objects.Length; i++)
        {
            if (!Utilities.IsValid(objects[i])) continue;
            FishingPole fishingPole = objects[i].GetComponentInChildren<FishingPole>();
            if (Utilities.IsValid(fishingPole))
            {
                fishingPole.reelHapticsEnabled = !toggle.isOn;
                PlayerData.SetBool(DataKeys.HapticsDisabled, toggle.isOn);
            }
        }
    }

    
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (Networking.GetOwner(gameObject) == player)
        {
            bool hapticsDisabled = PlayerData.GetBool(Networking.LocalPlayer, DataKeys.HapticsDisabled);
            toggle.isOn = hapticsDisabled;
        }
    }
}
