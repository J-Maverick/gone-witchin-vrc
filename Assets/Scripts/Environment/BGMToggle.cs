
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class BGMToggle : UdonSharpBehaviour
{

    public BGM bgm;
    public Toggle toggle;

    public void ToggleBGM()
    {
        if (!toggle.isOn)
        {
            bgm.EnableBGM();
        }
        else
        {
            bgm.DisableBGM();
        }
        PlayerData.SetBool(DataKeys.BGMDisabled, toggle.isOn);
    }

    
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (Networking.GetOwner(gameObject) == player)
        {
            bool bgmDisabled = PlayerData.GetBool(Networking.LocalPlayer, DataKeys.BGMDisabled);
            toggle.isOn = bgmDisabled;
        }
    }
}
