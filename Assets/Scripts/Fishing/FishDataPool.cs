
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Linq;

public class FishDataPool : UdonSharpBehaviour
{
    public FishData[] fishData;
    public Location testLocation;
    public Bait testBait = null;
    public Recipes recipes;
    public int recursions = 0;
    public int maxRecursions = 20;

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
            if (randomWeight < 0f) {
                FishData randomFish = fishData[i];
                if (randomFish.recipe != null && randomFish.recipe.unlocked) {
                    recursions++;
                    if (recursions > maxRecursions) {
                        Debug.LogFormat("{0}: Hit max recursions depth [{1}/{2}] looking for recipes, trying without bait", name, recursions, maxRecursions);
                        return GetRandomFishData(location, null);
                    }
                    // Picked a recipe but it's already unlocked, get another random fish
                    Debug.LogFormat("{0}: Tried to pick catchable recipe {1} but it was already unlocked, getting another fish", name, randomFish.name);
                    return GetRandomFishData(location, bait);
                }
                recursions = 0;
                return randomFish;
            }
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

}
