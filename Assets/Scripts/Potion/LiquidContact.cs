
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LiquidContact : UdonSharpBehaviour
{
    public PourableBottle bottle;
    public ReagentBottle reagentBottle = null;
    public Cauldron cauldron;

    private void OnParticleCollision(GameObject other)
    {
        //Debug.LogFormat("Particle Collision: {0}", other.name);
        if (other.layer == 22 && reagentBottle != null)
        cauldron.AddLiquid(reagentBottle);
    }
}
