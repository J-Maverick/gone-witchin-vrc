
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CatseyeElixir : PostProcessingPotion
{
    public override void OnDrink()
    {
        manager.Catseye();
    }
}
