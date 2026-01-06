
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class FishTooltip : UdonSharpBehaviour
{
    public Text fishNameText;
    public Text fishWeightText;
    public Color baseColor;
    public RawImage fishOilImage;
    public Color fishOilColor;
    public RawImage flamefinTearsImage;
    public Color flamefinTearsColor;
    public RawImage essenceOfWaterImage;
    public Color essenceOfWaterColor;
    public RawImage boiledBladderImage;
    public Color boiledBladderColor;
    public RawImage digestiveMudImage;
    public Color digestiveMudColor;
    public RawImage heartOfTroutImage;
    public Color heartOfTroutColor;
    public RawImage bioluminescentBileImage;
    public Color bioluminescentBileColor;
    public RawImage distilledDarknessImage;
    public Color distilledDarknessColor;
    public RawImage swiftfinSlimeImage;
    public Color swiftfinSlimeColor;
    public RawImage ocularJuiceImage;
    public Color ocularJuiceColor;
    public RawImage batfishGuanoImage;
    public Color batfishGuanoColor;
    public RawImage piranhaMilkImage;
    public Color piranhaMilkColor;
    public RawImage stinkyMucusImage;
    public Color stinkyMucusColor;
    public RawImage mishmashImage;
    public Color mishmashColor;
    public RawImage silveredSiltImage;
    public Color silveredSiltColor;
    public RawImage goldenGumboImage;
    public Color goldenGumboColor;
    public float goalScale = 0.001f;
    public float scaleFactor = 0.001f;
    public float scaleSpeed = 4f;
    bool isActive = false;
    bool isDeactivating = false;
    bool isScaling = false;
    public float activeTime = 5f;
    public float activeTimer = 0f;

    public void SetScale(Fish fish)
    {
        scaleFactor = goalScale / fish.transform.localScale.x;
    }

    public void UpdateTooltip(Fish fish)
    {
        SetFishName(fish.fishData);
        SetFishWeight(fish);
        SetColors(fish.fishData);
        SetScale(fish);
    }

    public void SetFishName(FishData fishData)
    {
        if (fishData == null) return;
        fishNameText.text = fishData.name;
    }

    public void SetFishWeight(Fish fish)
    {
        fishWeightText.text = string.Format("{0:0.#}", fish.weight);
    }

    public void SetBaseColor()
    {
        fishOilImage.color = baseColor;
        flamefinTearsImage.color = baseColor;
        essenceOfWaterImage.color = baseColor;
        boiledBladderImage.color = baseColor;
        digestiveMudImage.color = baseColor;
        heartOfTroutImage.color = baseColor;
        bioluminescentBileImage.color = baseColor;
        distilledDarknessImage.color = baseColor;
        swiftfinSlimeImage.color = baseColor;
        ocularJuiceImage.color = baseColor;
        batfishGuanoImage.color = baseColor;
        piranhaMilkImage.color = baseColor;
        stinkyMucusImage.color = baseColor;
        mishmashImage.color = baseColor;
        silveredSiltImage.color = baseColor;
        goldenGumboImage.color = baseColor;
    }

    public void SetColors(FishData fishData)
    {
        SetBaseColor();
        if (fishData == null) return;
        if (fishData.nFishOil > 0)
        {
            fishOilImage.color = fishOilColor;
        }
        if (fishData.nFlamefinTears > 0)
        {
            flamefinTearsImage.color = flamefinTearsColor;
        }
        if (fishData.nEssenceOfWater > 0)
        {
            essenceOfWaterImage.color = essenceOfWaterColor;
        }
        if (fishData.nBoiledBladder > 0)
        {
            boiledBladderImage.color = boiledBladderColor;
        }
        if (fishData.nDigestiveMud > 0)
        {
            digestiveMudImage.color = digestiveMudColor;
        }
        if (fishData.nHeartOfTrout > 0)
        {
            heartOfTroutImage.color = heartOfTroutColor;
        }
        if (fishData.nBioLuminescentBile > 0)
        {
            bioluminescentBileImage.color = bioluminescentBileColor;
        }
        if (fishData.nDistilledDarkness > 0)
        {
            distilledDarknessImage.color = distilledDarknessColor;
        }
        if (fishData.nSwiftfinSlime > 0)
        {
            swiftfinSlimeImage.color = swiftfinSlimeColor;
        }
        if (fishData.nOcularJuice > 0)
        {
            ocularJuiceImage.color = ocularJuiceColor;
        }
        if (fishData.nBatfishGuano > 0)
        {
            batfishGuanoImage.color = batfishGuanoColor;
        }
        if (fishData.nPiranhaMilk > 0)
        {
            piranhaMilkImage.color = piranhaMilkColor;
        }
        if (fishData.nStinkyMucus > 0)
        {
            stinkyMucusImage.color = stinkyMucusColor;
        }
        if (fishData.nMishMash > 0)
        {
            mishmashImage.color = mishmashColor;
        }
        if (fishData.nSilveredSilt > 0)
        {
            silveredSiltImage.color = silveredSiltColor;
        }
        if (fishData.nGoldenGumbo > 0)
        {
            goldenGumboImage.color = goldenGumboColor;
        }
    }

    public void Activate()
    {
        isActive = true;
        isDeactivating = false;
        isScaling = true;
        transform.localScale = Vector3.zero;
        activeTimer = 0f;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        if (!isActive) return;
        isDeactivating = true;
        isScaling = true;
    }

    private void Update()
    {
        if (isActive)
        {
            if (isScaling)
            {
                if (isDeactivating)
                {
                    transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, (scaleFactor * scaleSpeed) * Time.deltaTime);
                    if (transform.localScale.magnitude < 0.001f)
                    {
                        isActive = false;
                        isDeactivating = false;
                        isScaling = false;
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(scaleFactor, scaleFactor, scaleFactor), (scaleFactor * scaleSpeed) * Time.deltaTime);
                    if (transform.localScale.x == scaleFactor)
                    {
                        isScaling = false;
                    }
                }
            }
            // Ensure the tooltip is always facing the player's head
            // transform.LookAt(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position);
            // transform.Rotate(0, 180, 0); // Adjust rotation to face the player correctly
            
            transform.rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            // Update the active timer
            activeTimer += Time.deltaTime;
            if (activeTimer >= activeTime)
            {
                Deactivate();
            }
        }
    }
}
