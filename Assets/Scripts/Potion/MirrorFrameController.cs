using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum MirrorType
{
    LQ = 0,
    HQ = 1
}

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MirrorFrameController : UdonSharpBehaviour
{
    [UdonSynced] public bool frameActive = false;
    public bool mirrorActive = false;

    public GameObject mirrorParent;
    public GameObject mirrorHQ;
    public GameObject mirrorLQ;

    public BoxCollider playerCollider;

    public MirrorType type = MirrorType.LQ;

    private void Start()
    {
        playerCollider.enabled = false;
    }

    public void ButtonUsed(MirrorType newType)
    {
        type = newType;
        ToggleMirror();
    }

    public void ToggleMirror()
    {
        switch (type)
        {
            case MirrorType.HQ:
                mirrorHQ.SetActive(!mirrorHQ.activeSelf);
                mirrorLQ.SetActive(false);
                break;
            case MirrorType.LQ:
                mirrorLQ.SetActive(!mirrorLQ.activeSelf);
                mirrorHQ.SetActive(false);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!frameActive && other.gameObject.name == "MirrorFrameTrigger")
        {
            frameActive = true;
            mirrorParent.gameObject.SetActive(true);
            playerCollider.enabled = true;
            ToggleMirror();
            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if (frameActive && (!mirrorParent.gameObject.activeSelf || !playerCollider.enabled))
        {
            mirrorParent.gameObject.SetActive(true);
            playerCollider.enabled = true;
            ToggleMirror();
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (frameActive && player.isLocal)
        {
            if (!mirrorParent.gameObject.activeSelf)
            {
                mirrorParent.gameObject.SetActive(true);
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (frameActive && player.isLocal)
        {
            mirrorParent.gameObject.SetActive(false);
        }
    }

}
