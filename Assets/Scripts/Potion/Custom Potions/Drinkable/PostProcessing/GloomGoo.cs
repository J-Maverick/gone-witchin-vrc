
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GloomGoo : PostProcessingPotion
{
    public override void OnDrink()
    {
        manager.Gloom();
    }
}
