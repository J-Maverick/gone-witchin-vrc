
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MeshCollection : UdonSharpBehaviour
{
    public Mesh[] meshes;
    public SkinnedMeshRenderer skinnedMesh;
    void Update()
    {
        int randomIndex = Random.Range(0, meshes.Length);
        skinnedMesh.sharedMesh = meshes[randomIndex];
    }
}
