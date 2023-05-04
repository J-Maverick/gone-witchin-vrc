
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MirrorButton : UdonSharpBehaviour
{
    public MirrorFrameController mirrorFrame;
    public MirrorType type;

    public override void Interact()
    {
        mirrorFrame.ButtonUsed(type);
    }
}
