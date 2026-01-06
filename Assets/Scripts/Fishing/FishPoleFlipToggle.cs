
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class FishPoleFlipToggle : UdonSharpBehaviour
{
    public Toggle toggle;

    public void ToggleFlip()
    {
        var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
        for (int i = 0; i < objects.Length; i++)
        {
            if (!Utilities.IsValid(objects[i])) continue;
            FishingPole fishingPole = objects[i].GetComponentInChildren<FishingPole>();
            if (Utilities.IsValid(fishingPole))
            {
                PlayerData.SetBool(DataKeys.PoleFlipped, toggle.isOn);
                fishingPole.SetPoleFlip();
            }
        }
    }

    
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (Networking.GetOwner(gameObject) == player)
        {
            bool poleFlipped = PlayerData.GetBool(Networking.LocalPlayer, DataKeys.PoleFlipped);
            toggle.isOn = poleFlipped;
        }
    }
}
