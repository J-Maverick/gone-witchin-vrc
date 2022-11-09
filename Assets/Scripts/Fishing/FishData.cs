
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishData : UdonSharpBehaviour
{
    public Mesh mesh;
    public Color color = Color.white;
    public Color emissionColor = Color.black;

    [Space(10)]
    public float exhaustionMultiplier = 1f;
    public float forceMultiplier = 1f;
    public float minWeight = 1f;
    public float maxWeight = 100f;
    public float minScale = 1f;
    public float maxScale = 100f;

    [Space(10)]
    public bool foundInLake = false;
    public bool foundInCave = false;

    [Space(10)]
    public float lakeCatchChance = 0f;
    public float caveCatchChance = 0f;

    [Space(10)]
    public float apprenticeBaitBonus = 0f;
    public float journeymanBaitBonus = 0f;
    public float adeptBaitBonus = 0f;
    public float masterBaitBonus = 0f;
    public float goldenBaitBonus = 0f;

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
}
