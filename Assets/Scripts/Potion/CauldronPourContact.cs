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
        if (other.layer == 28 && cauldronPour != null && cauldronPour.bottleSnap.GetBottle().gameObject == other && cauldronPour.cauldron.matchingRecipe != null)
        {
            //if (Networking.GetOwner(other).isLocal && Networking.GetOwner(gameObject).isLocal)
            //{
                CheckBottleObject(other);
                targetBottle.AddLiquid(cauldronPour.cauldron.matchingRecipe.potion, cauldronPour.flow);
            //}
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

