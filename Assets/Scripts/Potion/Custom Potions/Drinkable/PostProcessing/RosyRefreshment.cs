
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RosyRefreshment : PostProcessingPotion
{
    public override void OnDrink()
    {
        manager.Rosy();
    }
}
