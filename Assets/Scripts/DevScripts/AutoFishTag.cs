using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoFishTag : MonoBehaviour
{
    public FishDataPool fishDataPool;

    [ContextMenu("Auto Tag Fish")]
    public void TagFish() {
        foreach (FishData fish in fishDataPool.fishData) {
            fish.tags = null;
            List<FishTag> fishTags = new List<FishTag>();
            MaterialTags(fish, fishTags);
            LocationTags(fish, fishTags);
            SizeTags(fish, fishTags);
            PersonalityTags(fish, fishTags);
            RarityTags(fish, fishTags);

            if (fishTags.Count > 0) {
                fish.tags = fishTags.ToArray();
            }
            else {
                fish.tags = null;
            }
        }
    } 

    public void MaterialTags(FishData fish, List<FishTag> fishTags) {
        if (fish.nFishOil > 0f) {
            fishTags.Add(FishTag.FishOil);
        }
        if (fish.nFlamefinTears > 0f) {
            fishTags.Add(FishTag.FlamefinTears);
        }
        if (fish.nEssenceOfWater > 0f) {
            fishTags.Add(FishTag.EssenceOfWater);
        }
        if (fish.nBoiledBladder > 0f) {
            fishTags.Add(FishTag.BoiledBladder);
        }
        if (fish.nDigestiveMud > 0f) {
            fishTags.Add(FishTag.DigestiveMud);
        }
        if (fish.nHeartOfTrout > 0f) {
            fishTags.Add(FishTag.HeartOfTrout);
        }
        if (fish.nBioLuminescentBile > 0f) {
            fishTags.Add(FishTag.BioLuminescentBile);
        }
        if (fish.nDistilledDarkness > 0f) {
            fishTags.Add(FishTag.DistilledDarkness);
        }
        if (fish.nSwiftfinSlime > 0f) {
            fishTags.Add(FishTag.SwiftfinSlime);
        }
        if (fish.nOcularJuice > 0f) {
            fishTags.Add(FishTag.OcularJuice);
        }
        if (fish.nBatfishGuano > 0f) {
            fishTags.Add(FishTag.BatfishGuano);
        }
        if (fish.nPiranhaMilk > 0f) {
            fishTags.Add(FishTag.PiranhaMilk);
        }
        if (fish.nStinkyMucus > 0f) {
            fishTags.Add(FishTag.StinkyMucus);
        }
        if (fish.nMishMash > 0f) {
            fishTags.Add(FishTag.MishMash);
        }
        if (fish.nSilveredSilt > 0f) {
            fishTags.Add(FishTag.SilveredSilt);
        }
        if (fish.nGoldenGumbo > 0f) {
            fishTags.Add(FishTag.GoldenGumbo);
        }
        if (fish.recipe != null) {
            fishTags.Add(FishTag.Recipe);
        }
    }

    public void LocationTags(FishData fish, List<FishTag> fishTags) {
        // if (fish.foundInLake) {
        //     fishTags.Add(FishTag.Lake);
        // }
        // if (fish.foundInCave) {
        //     fishTags.Add(FishTag.Cave);
        // }
        // if (fish.foundInLake && !fish.foundInCave) {
        //     fishTags.Add(FishTag.LakeOnly);
        // }
        // if (!fish.foundInLake && fish.foundInCave) {
        //     fishTags.Add(FishTag.CaveOnly);
        // }
    }
    public void SizeTags(FishData fish, List<FishTag> fishTags) {
        // Update if intending to actually use size tags
    }
    public void PersonalityTags(FishData fish, List<FishTag> fishTags) {
        // Update when personalities are implemented
    }
    public void RarityTags(FishData fish, List<FishTag> fishTags) {
    }

}
