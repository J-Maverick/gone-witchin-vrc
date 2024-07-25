using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class CatchRatesToCSV : MonoBehaviour
{
    public FishDataPool fishDataPool = null;
    public Water[] waters;
    public Bait[] baits;
    public string path = @"D:\Users\Joey\Documents\Unity\Projects\GoneWitchinHD\Assets\FishCatchRates.csv";

    [ContextMenu("Write to CSV")]
    public void WriteWeightsToCSV()
    {
        Debug.LogFormat("{0}: Writing to csv...", name);
        float t1 = Time.realtimeSinceStartup;
        using (StreamWriter sw = File.CreateText(path))
        {
            Dictionary<FishData, int> fishMap = new Dictionary<FishData, int>();
            foreach (FishData fishDataItem in fishDataPool.fishData) {
                fishMap[fishDataItem] = 0;
            }
            foreach (Water water in waters) {
                string firstLine = water.name;
                int allLength = water.common.Length + water.uncommon.Length + water.rare.Length + water.epic.Length + water.legendary.Length;
                FishData[] fishData = new FishData[allLength];
                int i = 0;
                foreach (FishData fish in water.common) {
                    fishData[i] = fish;
                    fishMap[fish] += 1;
                    i++;
                }
                foreach (FishData fish in water.uncommon) {
                    fishData[i] = fish;
                    fishMap[fish] += 1;
                    i++;
                }
                foreach (FishData fish in water.rare) {
                    fishData[i] = fish;
                    fishMap[fish] += 1;
                    i++;
                }
                foreach (FishData fish in water.epic) {
                    fishData[i] = fish;
                    fishMap[fish] += 1;
                    i++;
                }
                foreach (FishData fish in water.legendary) {
                    fishData[i] = fish;
                    fishMap[fish] += 1;
                    i++;
                }
                foreach (FishData fish in fishData)
                {
                    firstLine += ",";
                    firstLine += fish.name;
                }
                sw.WriteLine(firstLine);
        
                foreach (Bait bait in baits)
                {
                    float[] weights = new float[allLength];
                    float sumOfWeights = 0f;
                    i = 0;
                    foreach (FishData fish in water.common) {
                        float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Common): Rarity.Common;
                        weights[i] = weight;
                        sumOfWeights += weight;
                        i++;
                    }
                    foreach (FishData fish in water.uncommon) {
                        float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Uncommon): Rarity.Uncommon;
                        weights[i] = weight;
                        sumOfWeights += weight;
                        i++;
                    }
                    foreach (FishData fish in water.rare) {
                        float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Rare): Rarity.Rare;
                        weights[i] = weight;
                        sumOfWeights += weight;
                        i++;
                    }
                    foreach (FishData fish in water.epic) {
                        float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Epic): Rarity.Epic;
                        weights[i] = weight;
                        sumOfWeights += weight;
                        i++;
                    }
                    foreach (FishData fish in water.legendary) {
                        float weight = bait != null ? bait.GetCatchChance(fish, RarityEnum.Legendary): Rarity.Legendary;
                        weights[i] = weight;
                        sumOfWeights += weight;
                        i++;
                    }
                    string line = bait.name;
                    foreach (float weight in weights) line += "," + (100 * weight / sumOfWeights).ToString();
                    sw.WriteLine(line);
                }
                
                sw.WriteLine("");
            }
            string newLine = "N Usages";
            string lastLine = "";
            foreach (var item in fishMap) {
                newLine += "," + item.Key.name;
                lastLine += "," + item.Value;
            }
            sw.WriteLine(newLine);
            sw.WriteLine(lastLine);
        }
        Debug.LogFormat("{0}: Done writing to csv in {1}s", name, Time.realtimeSinceStartup - t1);
    }

    [ContextMenu("Give Fish ID's")]
    public void GiveFishIDs()
    {
        int ID = 0;
        foreach (FishData fish in fishDataPool.fishData)
        {
            ID += 1;
            fish.ID = ID;
        }
    }
}
