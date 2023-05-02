
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LovePotionTrigger : UdonSharpBehaviour
{
    public LovePotion lovePotion = null;

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (lovePotion != null) lovePotion.ActivateLove(player);
    }
}
