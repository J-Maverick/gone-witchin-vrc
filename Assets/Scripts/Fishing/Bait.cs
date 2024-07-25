
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Bait : UdonSharpBehaviour
{
    public BaitType type = BaitType.None;
    public TagModifier[] tagModifiers = null;
    public float bonus = 0f;
    public int castsPerBait = 5;
    public int craftAmount = 5;
    public int craftVariance = 1;
    public Mesh mesh;
    public Material material;
    

    public float UpdateCatchRateModifier(float catchRateModifier, FishTag[] fishTags, Water water) {
        if (fishTags != null && tagModifiers != null) {
            foreach (TagModifier tagModifier in tagModifiers) {
                catchRateModifier = tagModifier.UpdateCatchRateModifier(catchRateModifier, fishTags, water);
            }
        }
        return catchRateModifier;
    }

    public float UpdateCatchRateMod(float catchRateModifier, FishTag[] fishTags) {
        if (fishTags != null && tagModifiers != null) {
            foreach (TagModifier tagModifier in tagModifiers) {
                catchRateModifier = tagModifier.UpdateCatchRateMod(catchRateModifier, fishTags);
            }
        }
        return catchRateModifier;
    }

    
    public float GetCatchChance(FishData fishData, RarityEnum rarity) {
        float modifier = 1f;
        float catchChance = 0f;
        float bonus = 0f;
        
        switch (rarity) {
            case RarityEnum.Common:
                catchChance = Rarity.Common;
                break;
            case RarityEnum.Uncommon:
                catchChance = Rarity.Uncommon;
                break;
            case RarityEnum.Rare:
                catchChance = Rarity.Rare;
                break;
            case RarityEnum.Epic:
                catchChance = Rarity.Epic;
                break;
            case RarityEnum.Legendary:
                catchChance = Rarity.Legendary;
                break;
        }

        if (type == BaitType.Tag) {
            modifier = UpdateCatchRateMod(modifier, fishData.tags);
        }
        else {
            switch (type)
            {
                case BaitType.Apprentice:
                    if (rarity == RarityEnum.Uncommon) bonus = RarityBonus.Uncommon;
                    break;
                case BaitType.Journeyman:
                    if (rarity == RarityEnum.Rare) bonus = RarityBonus.Rare;
                    break;
                case BaitType.Adept:
                    if (rarity == RarityEnum.Epic) bonus = RarityBonus.Epic;
                    break;
                case BaitType.Master:
                    if (rarity == RarityEnum.Legendary) bonus = RarityBonus.Legendary;
                    break;
                // case BaitType.Golden:
                //     if (rarity == RarityEnum.Uncommon) bonus = goldenLakeBaitBonus;
                //     break;
            }
        }
        return bonus + catchChance * modifier;
    }
}
