using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class CatchRatesToCSV : MonoBehaviour
{
    public FishDataPool fishDataPool = null;
    public string path = @"C:\Users\Joey\Documents\Unity\Projects\GoneWitchinHD\Assets\FishCatchRates.csv";

    [ContextMenu("Write to CSV")]
    public void WriteWeightsToCSV()
    {
        Location[] locations = new Location[] { Location.lake, Location.cave };
        Bait[] baits = new Bait[] { Bait.none, Bait.apprentice, Bait.journeyman, Bait.adept, Bait.master, Bait.golden };

        using (StreamWriter sw = File.CreateText(path))
        {
            string firstLine = "";
            foreach (FishData fish in fishDataPool.fishData)
            {
                firstLine += ",";
                firstLine += fish.name;
            }
            sw.WriteLine(firstLine);
            foreach (Location location in locations)
            {
                foreach (Bait bait in baits)
                {
                    string line = bait.ToString() + "(" + location.ToString() + ")";
                    float[] weights = new float[fishDataPool.fishData.Length];
                    float sumOfWeights = 0f;
                    for (int i = 0; i < fishDataPool.fishData.Length; i++)
                    {
                        float weight = fishDataPool.fishData[i].GetCatchChance(location, bait);
                        weights[i] = weight;
                        sumOfWeights += weight;
                    }
                    foreach (float weight in weights) line += "," + (100 * weight / sumOfWeights).ToString();
                    sw.WriteLine(line);
                }
            }
        }
    }
}
