
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class ReagentTankFiller : UdonSharpBehaviour
{
    public ReagentTank[] tanks;
    private float fillMultiplier = 1000f;
    public VRCObjectPool pool;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 24 && Networking.GetOwner(other.gameObject).isLocal)
        {
            Fish fish = other.gameObject.GetComponent<Fish>();
            FillTank(fish);
        }
    }

    void FillTank(Fish fish)
    {
        Debug.LogFormat("{0}: Filling with fish: {1} | weight: {2}", name, fish.fishData.name, fish.weight);
        float fishMultiplier = fish.weight / fillMultiplier;
        Networking.SetOwner(Networking.GetOwner(fish.gameObject), pool.gameObject);
        foreach (ReagentTank tank in tanks)
        {
            Networking.SetOwner(Networking.GetOwner(fish.gameObject), tank.gameObject);
            switch (tank.reagent.ID)
            {
                case 0:
                    tank.fillLevel += fish.fishData.nFishOil * fishMultiplier;
                    break;
                case 1:
                    tank.fillLevel += fish.fishData.nFlamefinTears * fishMultiplier;
                    break;
                case 2:
                    tank.fillLevel += fish.fishData.nEssenceOfWater * fishMultiplier;
                    break;
                case 3:
                    tank.fillLevel += fish.fishData.nBoiledBladder * fishMultiplier;
                    break;
                case 4:
                    tank.fillLevel += fish.fishData.nDigestiveMud * fishMultiplier;
                    break;
                case 5:
                    tank.fillLevel += fish.fishData.nHeartOfTrout * fishMultiplier;
                    break;
                case 6:
                    tank.fillLevel += fish.fishData.nBioLuminescentBile * fishMultiplier;
                    break;
                case 7:
                    tank.fillLevel += fish.fishData.nDistilledDarkness * fishMultiplier;
                    break;
                case 8:
                    tank.fillLevel += fish.fishData.nSwiftfinSlime * fishMultiplier;
                    break;
                case 9:
                    tank.fillLevel += fish.fishData.nOcularJuice * fishMultiplier;
                    break;
                case 10:
                    tank.fillLevel += fish.fishData.nBatfishGuano * fishMultiplier;
                    break;
                case 11:
                    tank.fillLevel += fish.fishData.nPiranhaMilk * fishMultiplier;
                    break;
                case 12:
                    tank.fillLevel += fish.fishData.nStinkyMucus * fishMultiplier;
                    break;
                case 13:
                    tank.fillLevel += fish.fishData.nMishMash * fishMultiplier;
                    break;
                case 14:
                    tank.fillLevel += fish.fishData.nSilveredSilt * fishMultiplier;
                    break;
                case 15:
                    tank.fillLevel += fish.fishData.nGoldenGumbo * fishMultiplier;
                    break;
            }
            tank.Sync();
        }
        pool.Return(fish.gameObject);
    }
}
