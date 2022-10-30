
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fish : UdonSharpBehaviour
{
    public Transform fishBody;
    public Animator fishAnimator;
    public Transform lure;
    public Rigidbody lureRigidbody;
    public FishingPole fishingPole;

    public float fishWeight = 10f;
    public float fishForceMultiplier = 100f;
    private float forceMultiplier = 200f;
    public float maxAngleTowardsPlayer = 10f;

    public float maxWaitTime = 10f;
    public float minWaitTime = 5f;

    public float maxChangeTime = 12f;
    public float minChangeTime = 3f;

    public float fishWaitTime = 5f;
    public float fishTimer = 0f;
    public float directionChangeWaitTime = 5f;
    public float directionChangeTimer = 0f;

    public Vector3 fishforce = Vector3.zero;
    public Vector3 newDirection;
    public Vector3 forceDirection;

    public bool isFighting = false;
    public bool fishTimerIncremented = true;

    public float exhaustionRatio = 0.9f;
    public float exhaustion = 1f;

    public float exhaustionTime = 1f;
    public float exhaustionTimer = 0f;

    void Start()
    {
        RandomWaitTime();
    }

    void RandomWaitTime()
    {
        fishWaitTime = Random.Range(minWaitTime, maxWaitTime);
        fishTimer = 0f;
    }

    void RandomChangeTime()
    {
        directionChangeWaitTime = Random.Range(minWaitTime, maxWaitTime);
        directionChangeTimer = 0f;
    }

    void RandomDirection()
    {
        Vector3 distance = lure.position - fishingPole.transform.position;
        float angle = Mathf.Atan(distance.z / distance.x) * Mathf.Rad2Deg;
        if (distance.x < 0) angle += 180f;
        angle = Random.Range(-90f + angle - maxAngleTowardsPlayer, 90f + angle + maxAngleTowardsPlayer) * Mathf.Deg2Rad;
        
        newDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        newDirection = newDirection.normalized;
        forceMultiplier = fishForceMultiplier * Random.Range(0.8f, 1.2f);
        RandomChangeTime();
    }

    private void FixedUpdate()
    {
        if (fishingPole.fishOn && isFighting)
        {
            if (directionChangeTimer > directionChangeWaitTime) RandomDirection();
            forceDirection = Vector3.Lerp(forceDirection, newDirection, 0.01f);
            fishBody.rotation = Quaternion.LookRotation(-forceDirection);
            fishBody.position = transform.position;
            fishAnimator.SetFloat("SwimSpeed", forceMultiplier * exhaustion / 10f);
            lureRigidbody.AddForce(forceDirection * fishWeight * forceMultiplier * exhaustion);
            directionChangeTimer += Time.fixedDeltaTime;
            exhaustionTimer += Time.fixedDeltaTime;
            if (exhaustionTimer > exhaustionTime)
            {
                exhaustion *= exhaustionRatio;
                exhaustionTimer = 0f;
            }
        }
        else if (fishingPole.inWater)
        {
            if (fishTimer > fishWaitTime)
            {
                isFighting = true;
                RandomWaitTime();
                RandomDirection();
                forceDirection = newDirection;
                fishBody.rotation = Quaternion.LookRotation(-forceDirection);
                fishBody.position = transform.position;

                fishingPole.FishOn();
                exhaustion = 1f;
            }
            else fishTimer += Time.fixedDeltaTime;
            fishTimerIncremented = true;
        }
        else
        {
            if (isFighting || fishTimerIncremented)
            {
                RandomWaitTime();
                RandomChangeTime();
                isFighting = false;
                fishTimerIncremented = false;
            }
        }
    }
}
