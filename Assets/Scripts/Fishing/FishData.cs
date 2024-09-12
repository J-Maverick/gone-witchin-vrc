
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishData : UdonSharpBehaviour
{
    public int ID;
    public Mesh mesh = null;
    public Color color = Color.white;
    public Color emissionColor = Color.black;
    public Recipe recipe = null;

    [Space(10)]
    public float exhaustionMultiplier = 1f;
    public float forceMultiplier = 1f;
    public float minWeight = 1f;
    public float maxWeight = 100f;
    public float minScale = .1f;
    public float maxScale = 1f;

    [Space(10)]
    protected bool foundInLake = false;
    protected bool foundInCave = false;

    [Space(10)]
    protected float lakeCatchChance = 0f;
    protected float caveCatchChance = 0f;

    [Space(10)]
    protected float apprenticeLakeBaitBonus = 0f;
    protected float journeymanLakeBaitBonus = 0f;
    protected float adeptLakeBaitBonus = 0f;
    protected float masterLakeBaitBonus = 0f;
    protected float goldenLakeBaitBonus = 0f;
    [Space(5)]
    protected float apprenticeCaveBaitBonus = 0f;
    protected float journeymanCaveBaitBonus = 0f;
    protected float adeptCaveBaitBonus = 0f;
    protected float masterCaveBaitBonus = 0f;
    protected float goldenCaveBaitBonus = 0f;

    [Space(10)]
    public float nFishOil = 0f;
    public float nFlamefinTears = 0f;
    public float nEssenceOfWater = 0f;
    public float nBoiledBladder = 0f;
    public float nDigestiveMud = 0f;
    public float nHeartOfTrout = 0f;
    public float nBioLuminescentBile = 0f;
    public float nDistilledDarkness = 0f;
    public float nSwiftfinSlime = 0f;
    public float nOcularJuice = 0f;
    public float nBatfishGuano = 0f;
    public float nPiranhaMilk = 0f;
    public float nStinkyMucus = 0f;
    public float nMishMash = 0f;
    public float nSilveredSilt = 0f;
    public float nGoldenGumbo = 0f;

    public FishTag[] tags = null;

    public float GetCatchChance(Water water, Bait bait)
    {
        if (water.location == Location.Lake && foundInLake) return GetBaitModifierBonus(water, bait, lakeCatchChance) + GetLakeBonus(bait);

        else if (water.location == Location.Cave && foundInCave) return GetBaitModifierBonus(water, bait, caveCatchChance) + GetCaveBonus(bait);

        else return 0f;
    }

    public float GetBaitModifierBonus(Water water, Bait bait, float catchChance) {
        float modifier = 1f;
        if (bait != null && bait.type == BaitType.Tag) {
            modifier = bait.UpdateCatchRateModifier(modifier, tags, water);
        }
        return catchChance * modifier;
    }
    
    public float GetLakeBonus(Bait bait)
    {
        float bonus = 0f;
        if (bait != null) {
            switch (bait.type)
            {
                case BaitType.None:
                    bonus = 0f;
                    break;
                case BaitType.Apprentice:
                    bonus = apprenticeLakeBaitBonus;
                    break;
                case BaitType.Journeyman:
                    bonus = journeymanLakeBaitBonus;
                    break;
                case BaitType.Adept:
                    bonus = adeptLakeBaitBonus;
                    break;
                case BaitType.Master:
                    bonus = masterLakeBaitBonus;
                    break;
                case BaitType.Golden:
                    bonus = goldenLakeBaitBonus;
                    break;
            }
        }
        return bonus;
    }

    public float GetCaveBonus(Bait bait)
    {
        float bonus = 0f;
        if (bait != null) {
            switch (bait.type)
            {
                case BaitType.None:
                    bonus = 0f;
                    break;
                case BaitType.Apprentice:
                    bonus = apprenticeCaveBaitBonus;
                    break;
                case BaitType.Journeyman:
                    bonus = journeymanCaveBaitBonus;
                    break;
                case BaitType.Adept:
                    bonus = adeptCaveBaitBonus;
                    break;
                case BaitType.Master:
                    bonus = masterCaveBaitBonus;
                    break;
                case BaitType.Golden:
                    bonus = goldenCaveBaitBonus;
                    break;
            }
        }
        return bonus;
    }
}
