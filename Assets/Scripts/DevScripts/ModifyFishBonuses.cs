using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ModifyFishBonuses : MonoBehaviour
{
    public FishDataPool fishDataPool;
    [ContextMenu("Update Fish Bonuses")]
    public void UpdateFishBonuses() {
        foreach (FishData fish in fishDataPool.fishData) {
            if (fish.foundInLake)
            {
                fish.apprenticeLakeBaitBonus = (fish.lakeCatchChance + fish.apprenticeLakeBaitBonus) / fish.lakeCatchChance;
                fish.journeymanLakeBaitBonus = (fish.lakeCatchChance + fish.journeymanLakeBaitBonus) / fish.lakeCatchChance;
                fish.adeptLakeBaitBonus = (fish.lakeCatchChance + fish.adeptLakeBaitBonus) / fish.lakeCatchChance;
                fish.masterLakeBaitBonus = (fish.lakeCatchChance + fish.masterLakeBaitBonus) / fish.lakeCatchChance;
                fish.goldenLakeBaitBonus = (fish.lakeCatchChance + fish.goldenLakeBaitBonus) / fish.lakeCatchChance;

            }
            if (fish.foundInCave)
            { 
                fish.apprenticeCaveBaitBonus = (fish.caveCatchChance + fish.apprenticeCaveBaitBonus) / fish.caveCatchChance;
                fish.journeymanCaveBaitBonus = (fish.caveCatchChance + fish.journeymanCaveBaitBonus) / fish.caveCatchChance;
                fish.adeptCaveBaitBonus = (fish.caveCatchChance + fish.adeptCaveBaitBonus) / fish.caveCatchChance;
                fish.masterCaveBaitBonus = (fish.caveCatchChance + fish.masterCaveBaitBonus) / fish.caveCatchChance;
                fish.goldenCaveBaitBonus = (fish.caveCatchChance + fish.goldenCaveBaitBonus) / fish.caveCatchChance;
            }
        }
    }
}
