
using UdonSharp;
using System;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TagModifier : UdonSharpBehaviour
{
    public FishTag modifierTag;
    public float modifier = 1f;
    public bool inverse = false;
    public bool activeInLake = true;
    public bool activeInCave = true;
    public float lakeModifier = 1f;
    public float caveModifier = 1f;


    private float _UpdateCatchRateModifier(float catchRateModifier, Location location)
    {
        float mod = 1f;
        if (location == Location.Lake && activeInLake) {
            mod = modifier * lakeModifier;
        }
        else if (location == Location.Cave && activeInCave) {
            mod = modifier * caveModifier;
        }

        return catchRateModifier *= mod;
    }

    public float UpdateCatchRateModifier(float catchRateModifier, FishTag[] fishTags, Location location) {
        if (!inverse) {
            if (Array.IndexOf(fishTags, modifierTag) >= 0) {
                return _UpdateCatchRateModifier(catchRateModifier, location);
            }
        }
        else {
            if (Array.IndexOf(fishTags, modifierTag) < 0) {
                return _UpdateCatchRateModifier(catchRateModifier, location);
            }
        }
        return catchRateModifier;
    }
}


