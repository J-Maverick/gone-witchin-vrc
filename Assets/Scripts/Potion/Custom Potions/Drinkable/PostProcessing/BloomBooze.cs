
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BloomBooze : PostProcessingPotion
{
    public override void OnDrink()
    {
        manager.Bloom();
    }
}
