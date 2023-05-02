
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Eyedrops : PostProcessingPotion
{
    public override void OnDrink()
    {
        manager.Default();
    }
}
