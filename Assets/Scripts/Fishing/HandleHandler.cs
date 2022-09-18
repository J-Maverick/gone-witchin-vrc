
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HandleHandler : UdonSharpBehaviour
{
    public bool dropped = false;

    public override void OnDrop()
    {
        dropped = true;
    }
}
