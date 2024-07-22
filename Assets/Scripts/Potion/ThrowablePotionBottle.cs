
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ThrowablePotionBottle : Bottle
{
    public Renderer particleRenderer;
    public Material particleMaterial;
    public VRC_Pickup pickup;

    protected override void Start()
    {
        if (liquid != null) {
            potionColor = liquid.color;
            pickup.InteractionText = liquid.name;
            pickup.UseText = liquid.name;
        }
        particleMaterial = particleRenderer.material;
        particleMaterial.color = potionColor;

        base.Start();
    }
}
