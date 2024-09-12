
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class FishingZone : UdonSharpBehaviour
{
    public float innerRadius = 5f;
    public float outerRadius = 15f;
    public Water water;
    [UdonSynced] public bool zoneActive = false;
    [UdonSynced] public Vector3 zonePosition = new Vector3(0, 0, 0);
    public Animator animator;
    public bool handleOwnMovement = true;

    public FishZoneMode IsInZone(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        if (distance <= innerRadius)
        {
            return FishZoneMode.Inner;
        }
        else if (distance <= outerRadius)
        {
            return FishZoneMode.Outer;
        }
        else
        {
            return FishZoneMode.Outside;
        }
    }

    public override void OnPreSerialization()
    {
        zonePosition = transform.position;
    }

    public override void OnDeserialization()
    {
        animator.SetBool("ParticlesOn", zoneActive);
        if (handleOwnMovement) {
            transform.position = zonePosition;
        }
    }

    public void Activate(Vector3 position) {
        transform.position = position;
        zoneActive = true;
        animator.SetBool("ParticlesOn", true);
        RequestSerialization();
    }

    public void Activate() {
        zoneActive = true;
        animator.SetBool("ParticlesOn", true);
        RequestSerialization();
    }

    public void DeActivate() {
        zoneActive = false;
        animator.SetBool("ParticlesOn", false);
        RequestSerialization();
    }
}
