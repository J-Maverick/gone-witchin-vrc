
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class CreateMeshColliderStandins : UdonSharpBehaviour
{
    public FishDataPool fishDataPool;
    public GameObject standinPrefab;

    [ContextMenu("Create Mesh Colliders")]
    public void CreateMeshColliders()
    {
        //delete all children immediately
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        foreach (FishData fish in fishDataPool.fishData)
        {
            GameObject fishObject = Instantiate(standinPrefab, transform);
            fishObject.name = fish.name;
            fishObject.transform.localPosition = Vector3.zero;
            fishObject.transform.localRotation = Quaternion.identity;
            MeshCollider meshCollider = fishObject.GetComponent<MeshCollider>();
            meshCollider.sharedMesh = fish.mesh;
            meshCollider.convex = true;
            fishObject.SetActive(false);
        }
    }
}
