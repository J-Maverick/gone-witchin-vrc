
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class KaleidoBrew : PostProcessingPotion
{
    public override void OnDrink()
    {
        manager.Kaleido();
    }
}
