
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Bait : UdonSharpBehaviour
{
    public BaitType type = BaitType.None;
    public TagModifier[] tagModifiers = null;
    public int castsPerBait = 5;
    public int craftAmount = 5;
    public int craftVariance = 1;
    public Mesh mesh;
    public Material material;
    

    public float UpdateCatchRateModifier(float catchRateModifier, FishTag[] fishTags, Location location) {
        if (fishTags != null && tagModifiers != null) {
            foreach (TagModifier tagModifier in tagModifiers) {
                catchRateModifier = tagModifier.UpdateCatchRateModifier(catchRateModifier, fishTags, location);
            }
        }
        return catchRateModifier;
    }
}
