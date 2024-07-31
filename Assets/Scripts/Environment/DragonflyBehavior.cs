
using BestHTTP.SecureProtocol.Org.BouncyCastle.X509;
using System.Drawing;
using System.Security.AccessControl;
using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering;
using VRC.SDKBase;
using VRC.Udon;

public class DragonflyBehavior : UdonSharpBehaviour
{
    public float step = 1;
    public BoxCollider DragonflyBounds;
    public bool dragonflyEnabled;
    private Vector3 point;

    void Start()
    {
        transform.position = RandomPointInBounds(DragonflyBounds.bounds);
        WaitToMove();
    }

    public void WaitToMove() //recursive but not marked in udon as recursive, or something
    {
        SendCustomEventDelayedSeconds(nameof(PickPointAndMove), Random.Range(2, 10));
    }

    public void PickPointAndMove()
    {
        point = RandomPointInBounds(DragonflyBounds.bounds);
        Vector3 flatPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(flatPoint);
        dragonflyEnabled = true;
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void Update()
    {
        if (dragonflyEnabled)
        {
            transform.position = Vector3.MoveTowards(transform.position, point, step);
            if (transform.position == point)
            {
                WaitToMove();
                dragonflyEnabled = false;
            }
        }
    }
}
