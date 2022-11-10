
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fish : UdonSharpBehaviour
{
    public FishData fishData;
    public FishDataPool fishDataPool;

    public float weight = 1f;
    public float exhaustionMultiplier = 1f;
    public float forceMultiplier = 1f;

    public MeshRenderer meshRenderer;

    public bool randomize = false;

    public void GetRandomFish(Location location, Bait bait)
    {
        fishData = fishDataPool.GetRandomFishData(location, bait);
    }
    
    public void SetRandomSize()
    {
        float size = Random.Range(0f, 1f);
        weight = fishData.minWeight + size * (fishData.maxWeight - fishData.minWeight);
        transform.localScale = Vector3.one * (fishData.minScale + size * (fishData.maxScale - fishData.minScale));
    }

    private void Update()
    {
        if (randomize)
        {
            SetRandomSize();
            randomize = false;
        }
    }
}
