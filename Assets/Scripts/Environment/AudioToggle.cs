
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class AudioToggle : UdonSharpBehaviour
{
    public AudioSource[] audioSources;
    public Toggle toggle;

    public void ToggleAudio()
    {
        if (!toggle.isOn)
        {
            EnableAudio();
        }
        else
        {
            DisableAudio();
        }
        PlayerData.SetBool(DataKeys.AmbienceDisabled, toggle.isOn);
    }

    public void EnableAudio()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.mute = false;
        }
    }

    public void DisableAudio()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.mute = true;
        }
    }

    
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (Networking.GetOwner(gameObject) == player)
        {
            bool ambienceDisabled = PlayerData.GetBool(Networking.LocalPlayer, DataKeys.AmbienceDisabled);
            toggle.isOn = ambienceDisabled;
        }
    }
}
