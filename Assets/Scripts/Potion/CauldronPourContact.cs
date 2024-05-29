using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CauldronPourContact : UdonSharpBehaviour
{
    public CauldronPour cauldronPour = null;
    public PourableBottle targetBottle = null;
    public GameObject targetObject = null;

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == 28 && cauldronPour != null && cauldronPour.cauldron.matchingRecipe != null)
        {
            CheckBottleObject(other);
            if (Networking.GetOwner(other).isLocal)
            {
                targetBottle.AddLiquid(cauldronPour.cauldron.matchingRecipe.potion, cauldronPour.flow);
            }
            else {
                targetBottle.UpdateShaderFill();
            }
        }
    }

    private void CheckBottleObject(GameObject other)
    {
        if (targetObject != other)
        {
            targetObject = other;
            targetBottle = other.GetComponent<PourableBottle>();
        }
    }
}

