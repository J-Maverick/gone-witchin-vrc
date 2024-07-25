
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


    private float _UpdateCatchRateModifier(float catchRateModifier, Water water)
    {
        float mod = 1f;
        if (water.location == Location.Lake && activeInLake) {
            mod = modifier * lakeModifier;
        }
        else if (water.location == Location.Cave && activeInCave) {
            mod = modifier * caveModifier;
        }

        return catchRateModifier *= mod;
    }

    public float UpdateCatchRateModifier(float catchRateModifier, FishTag[] fishTags, Water water) {
        if (!inverse) {
            if (Array.IndexOf(fishTags, modifierTag) >= 0) {
                return _UpdateCatchRateModifier(catchRateModifier, water);
            }
        }
        else {
            if (Array.IndexOf(fishTags, modifierTag) < 0) {
                return _UpdateCatchRateModifier(catchRateModifier, water);
            }
        }
        return catchRateModifier;
    }

    public float UpdateCatchRateMod(float catchRateModifier, FishTag[] fishTags) {
        if (!inverse) {
            if (Array.IndexOf(fishTags, modifierTag) >= 0) {
                return modifier *= catchRateModifier;
            }
        }
        else {
            if (Array.IndexOf(fishTags, modifierTag) < 0) {
                return modifier *= catchRateModifier;
            }
        }
        return catchRateModifier;
    }

}


