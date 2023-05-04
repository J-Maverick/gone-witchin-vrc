
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum AvatarButtonType
{
    Active = 0,
    Left = 1,
    Right = 2
}
public class AvatarButton : UdonSharpBehaviour
{
    public AvatarFrame avatarFrame;
    public AvatarButtonType buttonType;

    public override void Interact()
    {
        avatarFrame.ButtonUsed(buttonType);
    }
}
