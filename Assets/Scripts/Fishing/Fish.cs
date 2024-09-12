
using UdonSharp;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
    public FishSync fishSync;

    public FishData fishData;
    public int fishID;
    public FishDataPool fishDataPool;
    public FishUnderwaterColors underwaterColors;

    public FishState state = FishState.reset;

    public float weight = 1f;
    public float size = 1f;

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

    public readonly float defaultSwimSpeed = 1f;
    
    public bool pickupEnabled = false;

    private float updateTime = 0f;

    public Mesh defaultMesh;
    public RandomAudioHandler randomAudioHandler;
    private float minWiggleTime = 0.5f;
    private float maxWiggleTime = 3f;
    private float minWiggleWait = 5f;
    private float maxWiggleWait = 60f;
    private float wiggleForceMultiplier = 2f;

    public void Start()
    {
        DisablePickup();
        material = meshRenderer.material;
    }

    public void DisablePickup()
    {
        Debug.LogFormat("{0}: Disable Pickup", name);
        meshCollider.enabled = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        pickup.pickupable = false;
        meshCollider.isTrigger = true;
        pickupEnabled = false;
    }

    public void EnablePickup()
    {
        meshCollider.enabled = true;
        pickup.pickupable = true;
        pickupEnabled = true;
    }

    public override void OnPickup()
    {
        Debug.LogFormat("{0}: OnPickup", name);
        rigidBody.constraints = RigidbodyConstraints.None;
        if (state != FishState.caught)
        {
            state = FishState.caught;
            Debug.LogFormat("{0}: Firing RandomWiggle", name);
            
            float randomTime = Random.Range(minWiggleWait, maxWiggleWait);
            Debug.LogFormat("{0}: RandomWiggle in {1}s", name, randomTime);
            SendCustomEventDelayedSeconds(nameof(RandomWiggle), randomTime);
            if (Networking.GetOwner(gameObject).isLocal)
            {
                fishSync.SendPickup();
            }
        };
        meshCollider.isTrigger = false;
    }

    public override void OnDrop()
    {
        meshCollider.isTrigger = false;
    }

    private void SetWaterLevel(Water water)
    {
        material = meshRenderer.material;
        
        if (water.location == Location.Lake)
        {
            Debug.LogFormat("{0}: Set water level for Lake", name);
            // material.SetFloat("_WaterLevel", water.transform.position.y + 0.001f);
            material.SetColor("_LightShadowColor", underwaterColors.lakeShadowColor);
            material.SetColor("_DarkShadowColor", underwaterColors.lakeShadowColor);
        }
        else if (water.location == Location.Cave)
        {
            Debug.LogFormat("{0}: Set water level for Cave", name);
            // material.SetFloat("_WaterLevel", water.transform.position.y + 0.001f);
            material.SetColor("_LightShadowColor", underwaterColors.caveShadowColor);
            material.SetColor("_DarkShadowColor", underwaterColors.caveShadowColor);
        }
        else {
            
            Debug.LogFormat("{0}: Failed to set water level", name);
        }
    }

    public void GetRandomFish(Water water, Water[] waters, Bait bait, float rodUpgradeMultiplier)
    {
        if (Networking.GetOwner(gameObject).isLocal)
        {
            fishData = water.GetRandomFishDataCombined(bait, waters);
            SetWaterLevel(water);
            exhaustionRatio = 1f - (fishData.exhaustionMultiplier * exhaustionReductionRatio * rodUpgradeMultiplier);
            SetRandomSize();
            fishID = fishData.ID;
        }
    }

    public void SetRandomSize()
    {
        size = Random.Range(0f, 1f);
        SetFishSizeProperties();
        Debug.LogFormat("{0}: Got random fish: {1} | size: {2} | weight: {3} | scale: {4}", name, fishData.name, size, weight, transform.localScale.x);
    }

    public void SetFishSizeProperties()
    {
        weight = fishData.minWeight + size * (fishData.maxWeight - fishData.minWeight);
        transform.localScale = Vector3.one * (fishData.minScale + size * (fishData.maxScale - fishData.minScale));
        material.SetFloat("_Scale", transform.localScale.x);
        if (fishData.mesh != null)
        {
            meshRenderer.sharedMesh = fishData.mesh;
            meshCollider.sharedMesh = fishData.mesh;
        }
        else {
            meshRenderer.sharedMesh = defaultMesh;
            meshCollider.sharedMesh = defaultMesh;
        }

        material.color = fishData.color;
        //meshRenderer.material = material;

        pickup.InteractionText = fishData.name;
        pickup.UseText = fishData.name;
    }


    public void Bite(Water water, Water[] waters, Bait bait, float rodUpgradeMultiplier)
    {
        GetRandomFish(water, waters, bait, rodUpgradeMultiplier);
        state = FishState.biting;
        exhaustion = 1f;
        animator.SetBool("Bite", true);
        material.SetFloat("_Speed", defaultSwimSpeed * fishData.forceMultiplier);
        // animator.SetFloat("SwimSpeed", defaultSwimSpeed * fishData.forceMultiplier);
        fishSync.Sync();
    }

    public bool TriggerFight()
    {
        animatorState = animator.GetCurrentAnimatorStateInfo(1);
        if (animatorState.IsTag("fighting"))
        {
            state = FishState.fighting;
            fishSync.Sync();
            return true;
        }
        else return false;
    }

    public void Fight()
    {
        exhaustionTimer += Time.fixedDeltaTime;
        if (exhaustionTimer > exhaustionTime)
        {
            // animator.SetFloat("SwimSpeed", defaultSwimSpeed * fishData.forceMultiplier * exhaustion);
            material.SetFloat("_Speed", defaultSwimSpeed * fishData.forceMultiplier * exhaustion);
            exhaustion *= exhaustionRatio;
            exhaustionTimer = 0f;
            fishSync.Sync();
        }

        if (exhaustion < catchThreshold && state == FishState.fighting)
        {
            Debug.LogFormat("{0}: Catchable", name);
            state = FishState.catchable;
            fishSync.Sync();
        }

        updateTime = FrequencySerialization(exhaustionTimer, updateTime);
    }

    public void Catch()
    {
        if (state == FishState.catchable)
        {
            state = FishState.catching;
            animator.SetBool("IsCaught", true);
            // material.SetFloat("_WaterLevel", -100000f);
            exhaustion = 0f;
            // animator.SetFloat("SwimSpeed", exhaustion);
            material.SetFloat("_Speed", exhaustion);
            EnablePickup();
            fishSync.Sync();
        }
    }

    public void Reset()
    {
        animator.SetBool("Bite", false);
        animator.SetBool("IsCaught", false);
        fishID = -1;
        fishData = null;
        state = FishState.reset;
        DisablePickup();
        fishSync.Sync();
    }

    public void Basket()
    {
        animator.SetBool("Bite", false);
        animator.SetBool("IsCaught", false);
        DisablePickup();
        state = FishState.basket;
        fishSync.Sync();
    }

    public void DeBasket()
    {
        transform.SetParent(null);
        state = FishState.caught;
        // animator.SetFloat("SwimSpeed", 0f);
        material.SetFloat("_Speed", 0f);
        animator.SetBool("Bite", true);
        animator.SetBool("IsCaught", true);
        // material.SetFloat("_WaterLevel", -100f);
        EnablePickup();
        fishSync.Sync();
    }

    public void Wiggle() {
        float randomIntensity = Random.Range(0.5f, 1.5f) * fishData.forceMultiplier;
        randomAudioHandler.slotZeroVolume = randomIntensity / 1.5f;
        randomAudioHandler.slotOneVolume = randomIntensity / 1.5f;
        randomAudioHandler.slotTwoVolume = randomIntensity / 1.5f;
        Vector3 randomRotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rigidBody.AddForce(Vector3.up * randomIntensity * wiggleForceMultiplier, ForceMode.Impulse);
        rigidBody.AddTorque(randomRotation * randomIntensity * wiggleForceMultiplier / 2f, ForceMode.Impulse);
        Debug.LogFormat("{0}: Wiggle at intensity: {1}", name, randomIntensity);
        material.SetFloat("_Wiggle", randomIntensity);
    }

    public void UnWiggle() {
        Debug.LogFormat("{0}: UnWiggle", name);
        material.SetFloat("_Wiggle", 0f);
    }

    public void RandomWiggle() {
        Debug.LogFormat("{0}: RandomWiggle", name);
        if (state == FishState.caught && gameObject.activeSelf) {
            float randomTime = Random.Range(minWiggleTime, maxWiggleTime);
            Debug.LogFormat("{0}: Firing Wiggle for {1}s", name, randomTime);
            if (!pickup.IsHeld & Networking.GetOwner(gameObject).isLocal) {
                Wiggle();
                if (randomTime < 1f) {
                    randomAudioHandler.PlaySlotZeroNetworked();
                }
                else if (randomTime < 2f) {
                    randomAudioHandler.PlaySlotOneNetworked();
                }
                else {
                    randomAudioHandler.PlaySlotTwoNetworked();
                }
            }
            SendCustomEventDelayedSeconds(nameof(RandomUnwiggle), randomTime);
        }
    }

    public void RandomUnwiggle() {
        Debug.LogFormat("{0}: RandomUnwiggle", name);
        UnWiggle();
        if (state == FishState.caught)
        {   
            float randomTime = Random.Range(minWiggleWait, maxWiggleWait);
            Debug.LogFormat("{0}: RandomWiggle in {1}s", name, randomTime);
            SendCustomEventDelayedSeconds(nameof(RandomWiggle), randomTime);
        }
    }

    public float GetForce()
    {
        return (75f + (size * 75f)) * fishData.forceMultiplier * exhaustion;
    }

    public float FrequencySerialization(float time, float nextUpdate, float freq=5f)
    {
        if (time >= nextUpdate)
        {
            fishSync.Sync();
            nextUpdate = time + (1f / freq);
        }
        return nextUpdate;
    }
}
