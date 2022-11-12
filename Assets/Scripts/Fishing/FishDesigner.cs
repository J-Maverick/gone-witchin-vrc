using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FishDesigner : MonoBehaviour
{
    public FishData fishData = null;
    public SkinnedMeshRenderer largeBody;
    public SkinnedMeshRenderer smallBody;
    public Transform largeFish;
    public Transform smallFish;

    private Material largeMaterial;
    private Material smallMaterial;

    private void Awake()
    {
        largeMaterial = largeBody.material;
        smallMaterial = smallBody.material;
    }

    void Update()
    {
        if (fishData != null)
        {
            largeMaterial.color = fishData.color;
            smallMaterial.color = fishData.color;

            if (fishData.mesh != null)
            {
                largeBody.sharedMesh = fishData.mesh;
                smallBody.sharedMesh = fishData.mesh;
            }

            largeFish.transform.localScale = Vector3.one * fishData.maxScale;
            smallFish.transform.localScale = Vector3.one * fishData.minScale;
        }
    }
}
