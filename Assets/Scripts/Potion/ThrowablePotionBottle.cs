
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ThrowablePotionBottle : Bottle
{
    public Renderer particleRenderer;
    public Material particleMaterial;
    public VRC_Pickup pickup;
    public PotionTooltip tooltip;

    protected override void Start()
    {
        if (liquid != null)
        {
            potionColor = liquid.color;
            pickup.InteractionText = liquid.name;
            pickup.UseText = liquid.name;
        }
        particleMaterial = particleRenderer.material;
        particleMaterial.color = potionColor;

        base.Start();
    }

    public override void OnPickup()
    {
        tooltip.UpdateTooltip(this);
        tooltip.Activate();
    }

    public override void OnDrop()
    {
        tooltip.Deactivate();
    }
}
