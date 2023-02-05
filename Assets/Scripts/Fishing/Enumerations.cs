
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum Bait
{
    none = 0,
    apprentice = 1,
    journeyman = 2,
    adept = 3,
    master = 4,
    golden = 5
}

public enum Location
{
    lake = 0,
    cave = 1
}


public static class LocationOffset
{
    public const float Lake = 0.001f;
    public const float Cave = -9.654f;
}