
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Water : UdonSharpBehaviour
{
    public Location location;

    [Space(10)]
    [Header("Location Randomizer Properties")]
    public Transform xMin;
    public Transform xMax;
    public Transform zMin;
    public Transform zMax;
    
    [Space(10)]
    [Header("Available Fish & Rarity Levels")]
    public FishData[] common;
    public FishData[] uncommon;
    public FishData[] rare;
    public FishData[] epic;
    public FishData[] legendary;

    // private FishData[] fishData;
    // private float[] catchRates;
    private int recursions = 0;
    private int maxRecursions = 20;

    // public void Start() {
    //     int allLength = common.Length + uncommon.Length + rare.Length + epic.Length + legendary.Length;
    //     fishData = new FishData[allLength];
    //     catchRates = new float[allLength];
    //     int i = 0;
    //     foreach (FishData fish in common) {
    //         fishData[i] = fish;
    //         catchRates[i] = Rarity.Common;
    //     }
    //     foreach (FishData fish in uncommon) {
    //         fishData[i] = fish;
    //         catchRates[i] = Rarity.Uncommon;
    //     }
    //     foreach (FishData fish in rare) {
    //         fishData[i] = fish;
    //         catchRates[i] = Rarity.Rare;
    //     }
    //     foreach (FishData fish in epic) {
    //         fishData[i] = fish;
    //         catchRates[i] = Rarity.Epic;
    //     }
    //     foreach (FishData fish in legendary) {
    //         fishData[i] = fish;
    //         catchRates[i] = Rarity.Legendary;
    //     }

    // }

    public FishData GetRandomFishData(Bait bait)
    { 
        int allLength = common.Length + uncommon.Length + rare.Length + epic.Length + legendary.Length;
        float[] weights = new float[allLength];
        float sumOfWeights = 0f;
        int i = 0;
        foreach (FishData fish in common) {
            float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Common): Rarity.Common;
            weights[i] = weight;
            sumOfWeights += weight;
            i++;
        }
        foreach (FishData fish in uncommon) {
            float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Uncommon): Rarity.Uncommon;
            weights[i] = weight;
            sumOfWeights += weight;
            i++;
        }
        foreach (FishData fish in rare) {
            float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Rare): Rarity.Rare;
            weights[i] = weight;
            sumOfWeights += weight;
            i++;
        }
        foreach (FishData fish in epic) {
            float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Epic): Rarity.Epic;
            weights[i] = weight;
            sumOfWeights += weight;
            i++;
        }
        foreach (FishData fish in legendary) {
            float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Legendary): Rarity.Legendary;
            weights[i] = weight;
            sumOfWeights += weight;
            i++;
        }

        float randomWeight = Random.Range(0f, sumOfWeights);

        int randomIndex = 0;
        for (int j = 0; j < weights.Length; ++j)
        {
            randomWeight -= weights[j];
            if (randomWeight < 0f) {
                randomIndex = j;
                break;
            }
        }
        FishData randomFish = null;
        if (randomIndex < common.Length) {
            randomFish = common[randomIndex];
        }
        else if (randomIndex < common.Length + uncommon.Length) {
            randomFish = uncommon[randomIndex - common.Length];
        }
        else if (randomIndex < common.Length + uncommon.Length + rare.Length) {
            randomFish = rare[randomIndex - common.Length - uncommon.Length];
        }
        else if (randomIndex < common.Length + uncommon.Length + rare.Length + epic.Length) {
            randomFish = epic[randomIndex - common.Length - uncommon.Length - rare.Length];
        }
        else if (randomIndex < common.Length + uncommon.Length + rare.Length + epic.Length + legendary.Length) {
            randomFish = legendary[randomIndex - common.Length - uncommon.Length - rare.Length - epic.Length];
        }

        if (randomFish != null && randomFish.recipe != null && randomFish.recipe.unlocked) {
            recursions++;
            if (recursions > maxRecursions) {
                Debug.LogFormat("{0}: Hit max recursions depth [{1}/{2}] looking for recipes, trying without bait", name, recursions, maxRecursions);
                return GetRandomFishData(null);
            }
            // Picked a recipe but it's already unlocked, get another random fish
            Debug.LogFormat("{0}: Tried to pick catchable recipe {1} but it was already unlocked, getting another fish", name, randomFish.name);
            return GetRandomFishData(bait);
        }
        recursions = 0;


        if (randomFish != null) {
            Debug.LogFormat("{0}: Got random fish {1}, Bait: {2}", name, randomFish.name, bait != null ? bait.name : "None");
        }
        else {
            Debug.LogFormat("{0}: Failed to get fish. Location: {1}, Bait: {2}", name, location.ToString(), bait != null ? bait.name : "None");
        }

        return randomFish;

    }

    public Vector3 GetRandomPointOnYPlane()
    {
        return new Vector3(Random.Range(xMin.position.x, xMax.position.x), transform.position.y, Random.Range(zMin.position.z, zMax.position.z));
    }

}
