
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReagentBottle : PourableBottle
{
    public Reagent reagent;
    public VRC_Pickup pickup;

    protected override void Start()
    {
        potionColor = reagent.color;
        pickup.InteractionText = reagent.name;
        base.Start();
    }
}
