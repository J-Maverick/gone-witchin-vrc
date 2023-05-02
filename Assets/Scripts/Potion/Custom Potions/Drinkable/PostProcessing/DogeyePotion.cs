
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DogeyePotion : PostProcessingPotion
{
    public override void OnDrink()
    {
        manager.Dogeye();
    }
}
