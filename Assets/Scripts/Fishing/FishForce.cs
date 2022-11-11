﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class FishForce : UdonSharpBehaviour
{
    public Transform lure;
    public Rigidbody lureRigidbody;

    public Transform hook;
    public FishingPole fishingPole;
    
    public float fishForceMultiplier = 100f;
    public float maxAngleTowardsPlayer = 10f;

    public float maxWaitTime = 10f;
    public float minWaitTime = 5f;

    public float maxChangeTime = 12f;
    public float minChangeTime = 3f;

    public float fishWaitTime = 5f;
    public float fishTimer = 0f;
    public float directionChangeWaitTime = 5f;
    public float directionChangeTimer = 0f;

    public Vector3 fishForce = Vector3.zero;
    public Vector3 newDirection;
    public Vector3 reverseDirection;
    public Vector3 forceDirection;

    public Transform fishBody = null;
    public Fish fish = null;

    public VRCObjectPool fishObjectPool;
    
    public Bait bait = Bait.none;

    public RandomPointGenerator randomPointGenerator;

    public float catchDistanceThreshold = 5f;
    
    //Networking.SetOwner(player, objectPool.gameObject);
    //    GameObject spawnedStation = objectPool.TryToSpawn();
    //Networking.SetOwner(player, spawnedStation);

    void Start()
    {
        RandomWaitTime();
    }

    public void GetFish()
    {
        if (fish == null)
        {
            Networking.SetOwner(Networking.GetOwner(gameObject), gameObject);
            GameObject spawnedFish = fishObjectPool.TryToSpawn();
            fish = spawnedFish.GetComponent<Fish>();
            Networking.SetOwner(Networking.GetOwner(gameObject), spawnedFish);
            fishBody = spawnedFish.transform;
        }
    }

    void RandomWaitTime()
    {
        fishWaitTime = Random.Range(minWaitTime, maxWaitTime);
        fishTimer = 0f;
    }

    void RandomChangeTime()
    {
        directionChangeWaitTime = Random.Range(minChangeTime, maxChangeTime);
        directionChangeTimer = 0f;
    }

    void RandomDirection()
    {
        //Vector3 distance = lure.position - fishingPole.transform.position;
        //float angle = Mathf.Atan(distance.z / distance.x) * Mathf.Rad2Deg;
        //if (distance.x < 0) angle += 180f;
        //angle = Random.Range(-90f + angle - maxAngleTowardsPlayer, 90f + angle + maxAngleTowardsPlayer) * Mathf.Deg2Rad;

        //newDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        newDirection = randomPointGenerator.GetRandomPointOnYPlane() - transform.position;
        newDirection = newDirection.normalized;
        RandomChangeTime();
    }

    private void Fight()
    {
        Debug.LogFormat("{0}: Fighting", name);
        fish.Fight();
        if (directionChangeTimer > directionChangeWaitTime) RandomDirection();
        forceDirection = Vector3.Lerp(forceDirection, newDirection, 0.01f);
        fishBody.rotation = Quaternion.LookRotation(forceDirection);
        fishBody.position = transform.position;
        fishForce = forceDirection * fishForceMultiplier * fish.GetForce();
        lureRigidbody.AddForce(fishForce);
        directionChangeTimer += Time.fixedDeltaTime;
    }

    private void Caught()
    {
        Debug.LogFormat("{0}: Caught", name);
        Vector3 rot = fishBody.rotation.eulerAngles;
        rot.x = -90f;
        fishBody.rotation = Quaternion.RotateTowards(fishBody.rotation, Quaternion.Euler(rot), 1);
        fishBody.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        fishBody.position = hook.position;
        fishForce = forceDirection * fishForceMultiplier * fish.GetForce();
        //hookRigidbody.AddForce(fishForce);
        directionChangeTimer += Time.fixedDeltaTime;
    }

    private void TriggerFight()
    {
        Debug.LogFormat("{0}: Triggering Fight", name);
        if (fish.TriggerFight())
        {
            fishingPole.FishOn();
            Fight();
            RandomWaitTime();
        }
    }

    private void Bite()
    {
        Debug.LogFormat("{0}: Bite", name);
        RandomDirection();
        forceDirection = Vector3.Lerp(forceDirection, newDirection, 0.01f);
        fishBody.rotation = Quaternion.LookRotation(forceDirection);
        fishBody.position = transform.position;
        fish.Bite(fishingPole.location, bait);
    }

    private void LockLure()
    {
        Vector3 pos = lure.position;
        pos.y = 0.02f;
        lure.SetPositionAndRotation(pos, Quaternion.Euler(-180, 0, 0));
        pos.y = -0.5f;
        hook.position = pos;
    }
    
    public void ResetFish()
    {
        Debug.LogFormat("{0}: Resetting", name);
        fish.Reset();
        RandomWaitTime();
        RandomChangeTime();
    }

    private void CheckBite()
    {
        if (fishTimer > fishWaitTime)
        {
            Bite();
        }
        else
        {
            Debug.LogFormat("{0}: Waiting for bite", name);
            fishTimer += Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != fishingPole.gameObject && fishingPole.fishOn)
        {
            RandomDirection();
        }
    }

    private void ResetOnFishOff()
    {
        if (!fishingPole.fishOn)
        {
            if (fish.state == FishState.fighting | fish.state == FishState.catchable | fish.state == FishState.catching) ResetFish();
        }
    }

    private void RemoveFish()
    {
        fish = null;
        fishBody = null;
        fishingPole.ResetLure();
    }

    private void FixedUpdate()
    {
        if (fishingPole.inWater)
        {
            GetFish();
            ResetOnFishOff();

            switch (fish.state)
            {
                case FishState.reset:
                    LockLure();
                    CheckBite();
                    break;
                case FishState.biting:
                    LockLure();
                    TriggerFight();
                    break;
                case FishState.fighting:
                    LockLure();
                    Fight();
                    break;
                case FishState.catchable:
                    LockLure();
                    Fight();
                    float distance = (lure.position - fishingPole.transform.position).magnitude;
                    if (distance < catchDistanceThreshold) fish.Catch();
                    break;
                case FishState.catching:
                    fishingPole.UnlockLure();
                    Caught();
                    break;
                case FishState.caught:
                    RemoveFish();
                    break;
            }
        }
        else if (fish != null && (fish.state != FishState.reset || fishTimer > 0f)) ResetFish();
    }
}
