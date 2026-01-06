
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LeverTooltipHandler : UdonSharpBehaviour
{
    public ReagentTank tank;
    public LeverTooltip tooltip;

    public override void OnPickup()
    {
        tooltip.UpdateTooltip(tank);
        tooltip.Activate();
    }

    public override void OnDrop()
    {
        tooltip.Deactivate();
    }

}
