
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum FishState
{
    reset = 0,
    biting = 1,
    fighting = 2,
    catchable = 3,
    catching = 4,
    caught = 5,
    basket = 6,
    pouring = 7
}

public class Fish : UdonSharpBehaviour
{
    public FishData fishData;
    public FishDataPool fishDataPool;

    public FishState state = FishState.reset;
    
    public float weight = 1f;

    public SkinnedMeshRenderer meshRenderer;
    public Material material;
    public MeshCollider meshCollider;
    public Rigidbody rigidBody;
    public VRC_Pickup pickup;

    public Animator animator;
    private AnimatorStateInfo animatorState;

    public float exhaustionReductionRatio = 0.04f;
    public float exhaustionRatio = 1f;
    public float exhaustion = 1f;

    public float exhaustionTime = 1f;
    public float exhaustionTimer = 0f;

    public float catchThreshold = 0.15f;

    private readonly float defaultSwimSpeed = 10f;

    public void Start()
    {
        DisablePickup();
        material = meshRenderer.material;
    }

    public void DisablePickup()
    {
        meshCollider.enabled = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        pickup.pickupable = false;
        meshCollider.isTrigger = true;
    }

    public void EnablePickup()
    {
        meshCollider.enabled = true;
        pickup.pickupable = true;
    }

    public override void OnPickup()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
        meshCollider.isTrigger = false;
        state = FishState.caught;
    }

    public void GetRandomFish(Location location, Bait bait)
    {
        fishData = fishDataPool.GetRandomFishData(location, bait);
        if (fishData.mesh != null) meshRenderer.sharedMesh = fishData.mesh;

        material.color = fishData.color;
        //meshRenderer.material = material;

        exhaustionRatio = 1f - (fishData.exhaustionMultiplier * exhaustionReductionRatio);
        pickup.InteractionText = fishData.name;
        SetRandomSize();
    }
    
    public void SetRandomSize()
    {
        float size = Random.Range(0f, 1f);
        weight = fishData.minWeight + size * (fishData.maxWeight - fishData.minWeight);
        transform.localScale = Vector3.one * (fishData.minScale + size * (fishData.maxScale - fishData.minScale));

        Debug.LogFormat("{0}: Got random fish: {1} | size: {2} | weight: {3} | scale: {4}", name, fishData.name, size, weight, transform.localScale.x);


    }

    public void Bite(Location location, Bait bait)
    {
        GetRandomFish(location, bait);
        state = FishState.biting;
        exhaustion = 1f;
        animator.SetBool("Bite", true);
        animator.SetFloat("SwimSpeed", defaultSwimSpeed * fishData.forceMultiplier);
    }

    public bool TriggerFight()
    {
        animatorState = animator.GetCurrentAnimatorStateInfo(1);
        if (animatorState.IsTag("fighting"))
        {
            state = FishState.fighting;
            return true;
        }
        else return false;
    }

    public void Fight()
    {
        exhaustionTimer += Time.fixedDeltaTime;
        if (exhaustionTimer > exhaustionTime)
        {
            animator.SetFloat("SwimSpeed", defaultSwimSpeed * fishData.forceMultiplier * exhaustion);
            exhaustion *= exhaustionRatio;
            exhaustionTimer = 0f;
        }

        if (exhaustion < catchThreshold && state == FishState.fighting) state = FishState.catchable;
    }

    public void Catch()
    {
        if (state == FishState.catchable)
        {
            state = FishState.catching;
            animator.SetBool("IsCaught", true);
            material.SetFloat("_WaterLevel", -100f);
            exhaustion = 0f;
            EnablePickup();
        }
    }

    public void Reset()
    {
        animator.SetBool("Bite", false);
        state = FishState.reset;
        DisablePickup();
    }

    public float GetForce()
    {
        return (75f + weight) * fishData.forceMultiplier * exhaustion;
    }

}
