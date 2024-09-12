
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishingPoleSpawnButton : UdonSharpBehaviour
{  
    public FishingPolePool fishingPolePool;
    public Transform spawnPoint;

    public override void Interact()
    {
        fishingPolePool.SummonRod(spawnPoint.position);
    }
}
