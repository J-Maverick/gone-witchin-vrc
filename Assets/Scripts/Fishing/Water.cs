
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Water : UdonSharpBehaviour
{
    public Location location;

    public Transform xMin;
    public Transform xMax;
    public Transform zMin;
    public Transform zMax;

    public Vector3 GetRandomPointOnYPlane()
    {
        return new Vector3(Random.Range(xMin.position.x, xMax.position.x), transform.position.y, Random.Range(zMin.position.z, zMax.position.z));
    }

}
