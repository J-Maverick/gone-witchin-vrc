
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Linq;

public class FishDataPool : UdonSharpBehaviour
{
    public FishData[] fishData;
    public bool testFish;
    public Location testLocation;
    public Bait testBait;
    public FishData randomFish;

    public FishData GetRandomFishData(Location location, Bait bait)
    {
        float[] weights = new float[fishData.Length];
        float sumOfWeights = 0f;
        for (int i = 0; i < fishData.Length; i++)
        {
            float weight = fishData[i].GetCatchChance(location, bait);
            weights[i] = weight;
            sumOfWeights += weight;
        }

        float randomWeight = Random.Range(0f, sumOfWeights);

        for (int i = 0; i < weights.Length; ++i)
        {
            randomWeight -= weights[i];
            if (randomWeight < 0f) return fishData[i];
        }

        return null;
    }

    public FishData GetFishByID(int fishID)
    {
        foreach (FishData fish in fishData)
        {
            if (fish.ID == fishID) return fish;
        }
        return null;
    }

    private void Update()
    {
        if (testFish)
        {
            float t1 = Time.realtimeSinceStartup;
            randomFish = GetRandomFishData(testLocation, testBait);
            testFish = false;
            float t2 = Time.realtimeSinceStartup;
            Debug.LogFormat("{0}: found random fish in {1}s", name, t2 - t1);
        }
    }
}
