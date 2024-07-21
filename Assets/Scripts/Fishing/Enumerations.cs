
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum BaitType
{
    None = 0,
    Apprentice = 1,
    Journeyman = 2,
    Adept = 3,
    Master = 4,
    Golden = 5,
    Tag = 6,
}

public enum FishTag 
{
    FishOil = 0,
    FlamefinTears = 1,
    EssenceOfWater = 2,
    BoiledBladder = 3,
    DigestiveMud = 4,
    HeartOfTrout = 5,
    BioLuminescentBile = 6,
    DistilledDarkness = 7,
    SwiftfinSlime = 8,
    OcularJuice = 9,
    BatfishGuano = 10,
    PiranhaMilk = 11,
    StinkyMucus = 12,
    MishMash = 13,
    SilveredSilt = 14,
    GoldenGumbo = 15,

    Lake = 100,
    Cave = 101,
    LakeOnly = 102,
    CaveOnly = 103,

    Small = 200,
    Medium = 201,
    Large = 202,

    Personality0 = 300,

    Common = 400,
    Uncommon = 401,
    Rare = 402,
    Legendary = 403,

    Recipe = 501,
}

public enum Location
{
    Lake = 0,
    Cave = 1
}


public static class LocationOffset
{
    public const float Lake = 0.001f;
    public const float Cave = -571.88f;
}