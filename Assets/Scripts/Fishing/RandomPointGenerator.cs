
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RandomPointGenerator : UdonSharpBehaviour
{
    public Transform xMin;
    public Transform xMax;
    public Transform zMin;
    public Transform zMax;

    public Vector3 GetRandomPointOnYPlane()
    {
        return new Vector3(Random.Range(xMin.position.x, xMax.position.x), 0f, Random.Range(zMin.position.z, zMax.position.z));
    }
}
