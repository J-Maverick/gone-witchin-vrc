
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StaticWobble : PotionWobble
{
    public override void Update() {
        Wobble();
    }
}
