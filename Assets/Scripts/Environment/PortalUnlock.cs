
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class PortalUnlock : UdonSharpBehaviour
{
    public Portal targetPortal;
    public GameObject portalParentObject;
    public AudioSource unlockSound;
    public bool isUnlocked = false;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            isUnlocked = PlayerData.GetBool(Networking.LocalPlayer, portalParentObject.name);
        }
        targetPortal.gameObject.SetActive(isUnlocked);
        targetPortal.targetPortal.gameObject.SetActive(isUnlocked);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal && !isUnlocked)
        {
            PlayAudio();
            isUnlocked = true;
            PlayerData.SetBool(portalParentObject.name, true);
            targetPortal.gameObject.SetActive(true);
            targetPortal.targetPortal.gameObject.SetActive(true);
        }
    }

    public void PlayAudio()
    {
        unlockSound.enabled = true;
        SendCustomEventDelayedSeconds(nameof(DisableAudio), unlockSound.clip.length + 0.1f);
    }

    public void DisableAudio()
    {
        if (unlockSound.isPlaying)
        {
            SendCustomEventDelayedSeconds(nameof(DisableAudio), unlockSound.clip.length + 0.1f);
            return;
        }
        unlockSound.enabled = false;
    }
}
