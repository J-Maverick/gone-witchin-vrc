
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishingPoleSpawnButton : UdonSharpBehaviour
{  
    public Transform spawnPoint;

    public void SpawnFishingPole()
    {
        var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
        for (int i = 0; i < objects.Length; i++)
        {
            if (!Utilities.IsValid(objects[i])) continue;
            FishingPole fishingPole = objects[i].GetComponentInChildren<FishingPole>();
            if (Utilities.IsValid(fishingPole)) {
                fishingPole.Teleport(spawnPoint);
                break;
            }
        }
    }
}
