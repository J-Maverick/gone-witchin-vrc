
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReagentBottle : PourableBottle
{
    public BottleTooltip tooltip;

    public override void UpdateLiquidProperties()
    {
        if (liquid != null)
        {
            potionColor = liquid.color;
            pickup.InteractionText = liquid.name;
            pickup.UseText = liquid.name;
            particleMaterial.color = potionColor;
            particleMaterial.SetColor("_EmissionColor", potionColor);
            shaderControl.SetStaticColor(potionColor);
        }
        tooltip.UpdateTooltip(this);
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
