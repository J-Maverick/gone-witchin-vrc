
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TankPourContact : UdonSharpBehaviour
{
    public ReagentTank tank = null;
    public ReagentBottle targetBottle = null;
    public GameObject targetObject = null;

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == 28 && tank != null)
        {
            //if (Networking.GetOwner(other).isLocal && Networking.GetOwner(gameObject).isLocal)
            //{
                CheckBottleObject(other);
                targetBottle.AddLiquid(tank.reagent, tank.flow);
            //}
        }
    }

    private void CheckBottleObject(GameObject other)
    {
        if (targetObject != other)
        {
            targetObject = other;
            targetBottle = other.GetComponent<ReagentBottle>();
        }
    }
}

