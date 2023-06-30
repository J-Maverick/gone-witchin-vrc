
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BottleSpawnLever : TriggerLever
{
    public BottleSpawner bottleSpawner = null;
    public override void Trigger()
    {
        bottleSpawner.Spawn();
    }
}
