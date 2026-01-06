
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class RigidBodyCorrector : UdonSharpBehaviour
{
    Vector3 correctedPosition;
    Quaternion correctedRotation;
    Rigidbody rb;
    private float updateTime = 1f;
    public FishingPole fishingPole;
    void Start()
    {
        correctedPosition = transform.localPosition;
        correctedRotation = transform.localRotation;
        updateTime += Random.Range(0.0f, 0.1f);
        rb = GetComponent<Rigidbody>();
        // CorrectPosition();
    }

    public void CorrectPosition() {
        transform.localPosition = correctedPosition;
        SendCustomEventDelayedSeconds(nameof(CorrectPosition), 0.5f);
    }

    public void ResetRigidbody() {
        transform.localPosition = correctedPosition;
        transform.localRotation = correctedRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void Update()
    {
        if (fishingPole.localPlayerClose) {
            transform.localPosition = correctedPosition;
        }
    }
}
