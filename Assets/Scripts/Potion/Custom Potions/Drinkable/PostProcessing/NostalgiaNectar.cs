
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NostalgiaNectar : PostProcessingPotion
{
    public override void OnDrink()
    {
        manager.Nostalgia();
    }
}
