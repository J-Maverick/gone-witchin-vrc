
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RespawnFollower : UdonSharpBehaviour
{
    public bool isFollowing = true;
    public RandomAudioHandler audioHandler;

    public void StopFollow()
    {
        isFollowing = false;
    }

    public void StartFollow() {
        isFollowing = true;
    }

    public void RespawnPlayer()
    {
        Networking.LocalPlayer.TeleportTo(transform.position, transform.rotation);
        Networking.LocalPlayer.SetVelocity(Vector3.zero);
        audioHandler.PlaySlotZero();
    }

    public void Update() {
        if (isFollowing && Networking.LocalPlayer.IsPlayerGrounded()) {
            transform.position = Networking.LocalPlayer.GetPosition();
            transform.rotation = Networking.LocalPlayer.GetRotation();
        }
    }
}
