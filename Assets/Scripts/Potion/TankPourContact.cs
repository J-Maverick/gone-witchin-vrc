
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
        Debug.LogFormat("Particle Collision: {0} | Layer: {1}", other.name, other.layer);
        if (other.layer == 28 && tank != null)
        {
            Debug.LogFormat("We doin the thing lol");
            if (Networking.GetOwner(other).isLocal && Networking.GetOwner(gameObject).isLocal)
            {
                Debug.LogFormat("We actually doin the thing lol");
                CheckBottleObject(other);
                targetBottle.AddReagent(tank.reagent, tank);
            }
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

