
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReagentBottle : PourableBottle
{
    public Reagent reagent = null;
    public VRC_Pickup pickup;
    private float fillAmount = 0.005f;
    public new BottleType type = BottleType.Reagent;

    protected override void Start()
    {
        UpdateReagentProperties();
        base.Start();
    }

    public void AddReagent(Reagent newReagent, ReagentTank tank)
    {
        if (reagent == null || fillLevel <= 0f)
        {
            reagent = newReagent;
            UpdateReagentProperties();
            AddFill(tank);
        }

        else if (reagent == newReagent)
        {
            AddFill(tank);
        }
    }

    public void UpdateReagentProperties()
    {
        if (reagent != null)
        {
            potionColor = reagent.color;
            pickup.InteractionText = reagent.name;
            particleMaterial.color = potionColor;
            shaderControl.SetStaticColor(potionColor);
        }
    }

    public void AddFill(ReagentTank tank)
    {
        if (fillLevel < 1f)
        {
            fillLevel += fillAmount * tank.flow;
        }
        if (fillLevel > 1f) fillLevel = 1f;

        UpdateShaderFill();
        syncObj.SetFill(fillLevel);
    }
}
