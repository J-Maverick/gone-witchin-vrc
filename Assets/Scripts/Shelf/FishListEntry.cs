
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class FishListEntry : UdonSharpBehaviour
{
    public FishData fishData;
    public FishLogEntry fishLogEntry;
    public FishLogEntrySynced fishLogEntrySynced;
    public TMP_Text fishNameText;
    public TMP_Text locationText;
    public TMP_Text fishSizeText;
    public GameObject largeGoldCrown;
    public GameObject largeSilverCrown;
    public GameObject largePBGoldCrown;
    public GameObject largePBSilverCrown;
    public GameObject smallGoldCrown;
    public GameObject smallSilverCrown;
    public GameObject smallPBGoldCrown;
    public GameObject smallPBSilverCrown;

    public void UpdateText() 
    {
        if (fishLogEntry.largestCaught > 0f || fishLogEntrySynced.largestCaught > 0f || fishLogEntrySynced.smallestCaught > -.5f || fishLogEntry.smallestCaught > -.5f)
        {
            fishNameText.text = fishData.name;
        }
        else
        {
            fishNameText.text = "???";
            return;
        }
        
        float largestWeight = fishData.minWeight + fishLogEntrySynced.largestCaught * (fishData.maxWeight - fishData.minWeight);
        float smallestWeight = fishData.minWeight + fishLogEntrySynced.smallestCaught * (fishData.maxWeight - fishData.minWeight);
        string largestPBWeight = fishLogEntry.largestCaught > 0 ? string.Format("{0:0.#}", fishData.minWeight + fishLogEntry.largestCaught * (fishData.maxWeight - fishData.minWeight)) : "";
        string smallestPBWeight = fishLogEntry.smallestCaught > -1 ? string.Format("{0:0.#}", fishData.minWeight + fishLogEntry.smallestCaught * (fishData.maxWeight - fishData.minWeight)) : "";

        UpdateCrowns();
        fishSizeText.text = string.Format(
            @"Largest: {0:0.#} ({1})
          PB: {2:0.#} 

Smallest: {3:0.#} ({4})
            PB: {5:0.#} ", largestWeight, fishLogEntrySynced.largestPlayerName, largestPBWeight, smallestWeight, fishLogEntrySynced.smallestPlayerName, smallestPBWeight);
        
    }

    public void UpdateCrowns()
    {
        largeGoldCrown.SetActive(false);
        largeSilverCrown.SetActive(false);
        largePBGoldCrown.SetActive(false);
        largePBSilverCrown.SetActive(false);
        smallGoldCrown.SetActive(false);
        smallSilverCrown.SetActive(false);
        smallPBGoldCrown.SetActive(false);
        smallPBSilverCrown.SetActive(false);

        if (fishLogEntry.largestCaught > 0.95f) {
            largePBGoldCrown.SetActive(true);
        }
        else if (fishLogEntry.largestCaught > 0.8f) {
            largePBSilverCrown.SetActive(true);
        }

        if (fishLogEntrySynced.largestCaught > 0.95f) {
            largeGoldCrown.SetActive(true);
        }
        else if (fishLogEntrySynced.largestCaught > 0.8f) {
            largeSilverCrown.SetActive(true);
        }

        if (fishLogEntry.smallestCaught < 0f) {}
        else if (fishLogEntry.smallestCaught < 0.05f) {
            smallPBGoldCrown.SetActive(true);
        }
        else if (fishLogEntry.smallestCaught < 0.2f) {
            smallPBSilverCrown.SetActive(true);
        }

        if (fishLogEntrySynced.smallestCaught < 0f) {}
        else if (fishLogEntrySynced.smallestCaught < 0.05f) {
            smallGoldCrown.SetActive(true);
        }
        else if (fishLogEntrySynced.smallestCaught < 0.2f) {
            smallSilverCrown.SetActive(true);
        }
    }
}
