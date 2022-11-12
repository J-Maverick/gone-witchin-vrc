
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

    public Vector3 GetRandomPointOnYPlane(float y)
    {
        return new Vector3(Random.Range(xMin.position.x, xMax.position.x), y, Random.Range(zMin.position.z, zMax.position.z));
    }
}
