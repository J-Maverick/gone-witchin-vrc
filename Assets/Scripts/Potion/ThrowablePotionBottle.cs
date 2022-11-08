
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ThrowablePotionBottle : Bottle
{
    public Potion potion;
    public Renderer particleRenderer;
    public Material particleMaterial;
    public VRC_Pickup pickup;

    protected override void Start()
    {
        potionColor = potion.color;
        particleMaterial = particleRenderer.material;
        particleMaterial.color = potionColor;

        pickup.InteractionText = potion.name;
        base.Start();
    }
}
