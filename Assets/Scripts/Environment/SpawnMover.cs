
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SpawnMover : UdonSharpBehaviour
{
    public Transform sceneDescriptor;
    public Transform respawnPoint;

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        sceneDescriptor.transform.position = respawnPoint.position;
        sceneDescriptor.transform.rotation = respawnPoint.rotation;
    }

}
