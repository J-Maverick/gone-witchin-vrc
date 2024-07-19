
using System.Runtime.CompilerServices;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DevBottle : UdonSharpBehaviour
{
    public LiquidMaterial liquid;
    public PourableBottle bottle;
    public BottleCollision bottleCollision;
    
    [ContextMenu("Respawn")]
    public void Respawn() {
        bottleCollision.Respawn();
    }

    [ContextMenu("Set Liquid")] 
    public void SetLiquid() {
        bottle.liquid = liquid;
        bottle.UpdateLiquidProperties();
        bottle.SetFill(1f);
    }
}
