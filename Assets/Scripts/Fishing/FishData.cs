
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

    [Space(10)]
    public float exhaustionMultiplier = 1f;
    public float forceMultiplier = 1f;
    public float minWeight = 1f;
    public float maxWeight = 100f;
    public float minScale = .1f;
    public float maxScale = 1f;

    [Space(10)]
    public bool foundInLake = false;
    public bool foundInCave = false;

    [Space(10)]
    public float lakeCatchChance = 0f;
    public float caveCatchChance = 0f;

    [Space(10)]
    public float apprenticeLakeBaitBonus = 0f;
    public float journeymanLakeBaitBonus = 0f;
    public float adeptLakeBaitBonus = 0f;
    public float masterLakeBaitBonus = 0f;
    public float goldenLakeBaitBonus = 0f;
    [Space(5)]
    public float apprenticeCaveBaitBonus = 0f;
    public float journeymanCaveBaitBonus = 0f;
    public float adeptCaveBaitBonus = 0f;
    public float masterCaveBaitBonus = 0f;
    public float goldenCaveBaitBonus = 0f;

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

    public float GetCatchChance(Location location, Bait bait)
    {
        if (location == Location.lake && foundInLake) return lakeCatchChance + GetLakeBonus(bait);

        else if (location == Location.cave && foundInCave) return caveCatchChance + GetCaveBonus(bait);

        else return 0f;
    }

    public float GetLakeBonus(Bait bait)
    {
        float bonus = 0f;
        switch (bait)
        {
            case Bait.none:
                bonus = 0f;
                break;
            case Bait.apprentice:
                bonus = apprenticeLakeBaitBonus;
                break;
            case Bait.journeyman:
                bonus = journeymanLakeBaitBonus;
                break;
            case Bait.adept:
                bonus = adeptLakeBaitBonus;
                break;
            case Bait.master:
                bonus = masterLakeBaitBonus;
                break;
            case Bait.golden:
                bonus = goldenLakeBaitBonus;
                break;
        }
        return bonus;
    }

    public float GetCaveBonus(Bait bait)
    {
        float bonus = 0f;
        switch (bait)
        {
            case Bait.none:
                bonus = 0f;
                break;
            case Bait.apprentice:
                bonus = apprenticeCaveBaitBonus;
                break;
            case Bait.journeyman:
                bonus = journeymanCaveBaitBonus;
                break;
            case Bait.adept:
                bonus = adeptCaveBaitBonus;
                break;
            case Bait.master:
                bonus = masterCaveBaitBonus;
                break;
            case Bait.golden:
                bonus = goldenCaveBaitBonus;
                break;
        }
        return bonus;
    }
}
