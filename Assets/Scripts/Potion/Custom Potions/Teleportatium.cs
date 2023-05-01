
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Teleportatium : ShatterEffect
{
    public override void OnShatter()
    {
        VRCPlayerApi owner = Networking.GetOwner(gameObject);
        owner.TeleportTo(transform.position, Quaternion.LookRotation(transform.position - owner.GetPosition()));
    }
}
