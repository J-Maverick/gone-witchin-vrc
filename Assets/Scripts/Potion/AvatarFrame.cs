
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class AvatarFrame : UdonSharpBehaviour
{
    [UdonSynced] public bool frameActive = false;
    public bool avatarsActive = false;
    public int activePage = 0;
    public GameObject toggleButton;
    public GameObject navigationButtons;
    public GameObject[] pages;

    public void ButtonUsed(AvatarButtonType buttonType)
    {
        switch (buttonType)
        {
            case AvatarButtonType.Active:
                ToggleAvatars();
                break;
            case AvatarButtonType.Left:
                PreviousPage();
                break;
            case AvatarButtonType.Right:
                NextPage();
                break;
        }
    }

    public void PreviousPage()
    {
        pages[activePage].SetActive(false);
        activePage--;
        if (activePage <= 0) activePage = pages.Length - 1;
        pages[activePage].SetActive(true);
    }

    public void NextPage()
    {
        pages[activePage].SetActive(false);
        activePage++;
        if (activePage >= pages.Length) activePage = 0;
        pages[activePage].SetActive(true);
    }

    public void ToggleAvatars()
    {
        avatarsActive = !avatarsActive;
        pages[activePage].SetActive(avatarsActive);
        navigationButtons.SetActive(avatarsActive);
    }
      

    private void OnTriggerEnter(Collider other)
    {
    if (!frameActive && other.gameObject.name == "AvatarPedestalTrigger")
        {
            frameActive = true;
            toggleButton.SetActive(true);
            ToggleAvatars();
        }
    }

    public override void OnDeserialization()
    {
        if (frameActive && !toggleButton.gameObject.activeSelf)
        {
            toggleButton.gameObject.SetActive(true);
            ToggleAvatars();
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }

}
