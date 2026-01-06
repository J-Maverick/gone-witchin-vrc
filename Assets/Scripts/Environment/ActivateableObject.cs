
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ActivateableObject : UdonSharpBehaviour
{
    [UdonSynced] public bool isActive = false;

    public void Start()
    {
        gameObject.SetActive(isActive);
    }

    public void Activate()
    {
        isActive = true;
        gameObject.SetActive(true);
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        gameObject.SetActive(isActive);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }


}
