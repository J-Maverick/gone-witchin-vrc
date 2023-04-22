
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum BottleType
{
    None = 0,
    Reagent = 1,
    Potion = 2
}

public class Bottle : UdonSharpBehaviour
{
    public Color potionColor;
    public PotionWobble shaderControl = null;
    public float fillLevel = 0f;
    public BottleType type = BottleType.None;

    protected virtual void Start()
    {
        shaderControl = GetComponent<PotionWobble>();
        shaderControl.SetStaticColor(potionColor);
    }

}
