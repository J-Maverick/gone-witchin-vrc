
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Bottle : UdonSharpBehaviour
{
    public Color potionColor;
    public PotionWobble shaderControl = null;

    protected virtual void Start()
    {
        shaderControl = GetComponent<PotionWobble>();
        shaderControl.SetStaticColor(potionColor);
    }

}
