
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class ReagentTankFiller : UdonSharpBehaviour
{
    public ReagentTank[] tanks;
    public float sizeQuotient = 10f;
    public float weightQuotient = 1000f;
    public VRCObjectPool pool;
    public Animator animator;
    public RandomAudioHandler audioHandler;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 24)
        {
            Fish fish = other.gameObject.GetComponent<Fish>();
            if (fish != null) {
                fish.state = FishState.caught;
                if (Networking.GetOwner(other.gameObject).isLocal) {
                    FillTank(fish);
                }
            }
        }
    }

    void FillTank(Fish fish)
    {
        Debug.LogFormat("{0}: Filling with fish: {1} | weight: {2}", name, fish.fishData.name, fish.weight);
        float fishMultiplier = (fish.weight / weightQuotient) + (fish.size / sizeQuotient);
        Networking.SetOwner(Networking.GetOwner(fish.gameObject), pool.gameObject);
        foreach (ReagentTank tank in tanks)
        {
            Networking.SetOwner(Networking.GetOwner(fish.gameObject), tank.gameObject);
            float fillUp = 0f;
            switch (tank.reagent.ID)
            {
                case 0:
                    fillUp = fish.fishData.nFishOil * fishMultiplier;
                    break;
                case 1:
                    fillUp = fish.fishData.nFlamefinTears * fishMultiplier;
                    break;
                case 2:
                    fillUp = fish.fishData.nEssenceOfWater * fishMultiplier;
                    break;
                case 3:
                    fillUp = fish.fishData.nBoiledBladder * fishMultiplier;
                    break;
                case 4:
                    fillUp = fish.fishData.nDigestiveMud * fishMultiplier;
                    break;
                case 5:
                    fillUp = fish.fishData.nHeartOfTrout * fishMultiplier;
                    break;
                case 6:
                    fillUp = fish.fishData.nBioLuminescentBile * fishMultiplier;
                    break;
                case 7:
                    fillUp = fish.fishData.nDistilledDarkness * fishMultiplier;
                    break;
                case 8:
                    fillUp = fish.fishData.nSwiftfinSlime * fishMultiplier;
                    break;
                case 9:
                    fillUp = fish.fishData.nOcularJuice * fishMultiplier;
                    break;
                case 10:
                    fillUp = fish.fishData.nBatfishGuano * fishMultiplier;
                    break;
                case 11:
                    fillUp = fish.fishData.nPiranhaMilk * fishMultiplier;
                    break;
                case 12:
                    fillUp = fish.fishData.nStinkyMucus * fishMultiplier;
                    break;
                case 13:
                    fillUp = fish.fishData.nMishMash * fishMultiplier;
                    break;
                case 14:
                    fillUp = fish.fishData.nSilveredSilt * fishMultiplier;
                    break;
                case 15:
                    fillUp = fish.fishData.nGoldenGumbo * fishMultiplier;
                    break;
            }
            
            tank.fillLevel += fillUp; 
            animator.SetFloat(tank.reagent.name, fillUp == 0f || fillUp > 0.015f ? fillUp * 10f : 0.15f);
            tank.Sync();
        }

        if (fish.fishData.recipe != null) {
            fish.fishData.recipe.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Recipe.Unlock));
        }
        animator.SetBool("PlayParticles", false);
        audioHandler.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(audioHandler.PlaySlotZero));
        SendCustomEventDelayedSeconds(nameof(PlayParticles), 0.2f);
        SendCustomEventDelayedSeconds(nameof(EndParticles), 5f);
        fish.fishSync.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(fish.fishSync.UpdateFishLog));
        fish.tooltip.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(fish.tooltip.Deactivate));
        pool.Return(fish.gameObject);
    }

    public void FillTankForLoad(FishData fishData, float size)
    {
        float weight = 0f;
        if (size > 0f) {
            weight = fishData.minWeight + size * (fishData.maxWeight - fishData.minWeight);
        }
        Debug.LogFormat("{0}: Filling with fish: {1} | weight: {2}", name, fishData.name, weight);
        float fishMultiplier = (weight / weightQuotient) + (size / sizeQuotient);
        foreach (ReagentTank tank in tanks)
        {
            Networking.SetOwner(Networking.GetOwner(gameObject), tank.gameObject);
            float fillUp = 0f;
            switch (tank.reagent.ID)
            {
                case 0:
                    fillUp = fishData.nFishOil * fishMultiplier;
                    break;
                case 1:
                    fillUp = fishData.nFlamefinTears * fishMultiplier;
                    break;
                case 2:
                    fillUp = fishData.nEssenceOfWater * fishMultiplier;
                    break;
                case 3:
                    fillUp = fishData.nBoiledBladder * fishMultiplier;
                    break;
                case 4:
                    fillUp = fishData.nDigestiveMud * fishMultiplier;
                    break;
                case 5:
                    fillUp = fishData.nHeartOfTrout * fishMultiplier;
                    break;
                case 6:
                    fillUp = fishData.nBioLuminescentBile * fishMultiplier;
                    break;
                case 7:
                    fillUp = fishData.nDistilledDarkness * fishMultiplier;
                    break;
                case 8:
                    fillUp = fishData.nSwiftfinSlime * fishMultiplier;
                    break;
                case 9:
                    fillUp = fishData.nOcularJuice * fishMultiplier;
                    break;
                case 10:
                    fillUp = fishData.nBatfishGuano * fishMultiplier;
                    break;
                case 11:
                    fillUp = fishData.nPiranhaMilk * fishMultiplier;
                    break;
                case 12:
                    fillUp = fishData.nStinkyMucus * fishMultiplier;
                    break;
                case 13:
                    fillUp = fishData.nMishMash * fishMultiplier;
                    break;
                case 14:
                    fillUp = fishData.nSilveredSilt * fishMultiplier;
                    break;
                case 15:
                    fillUp = fishData.nGoldenGumbo * fishMultiplier;
                    break;
            }
            tank.fillLevel += fillUp; 
            animator.SetFloat(tank.reagent.name, fillUp == 0f || fillUp > 0.015f ? fillUp * 10f : 0.15f);
            tank.Sync();
        }
    }

    public void PlayParticles() {
        animator.SetBool("PlayParticles", true);
    }

    public void EndParticles() {
        animator.SetBool("PlayParticles", false);
    }
}
