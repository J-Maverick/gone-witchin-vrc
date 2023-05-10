
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UseablePotion : DrinkablePotion
{
    public UseEffect useEffect;

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnUse");
    }

    public void OnUse()
    {
        useEffect.OnUse();
    }
}
