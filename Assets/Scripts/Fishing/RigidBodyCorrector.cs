
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class RigidBodyCorrector : UdonSharpBehaviour
{
    Vector3 correctedPosition;
    private float updateTime = 1f;
    public FishingPole fishingPole;
    void Start()
    {
        correctedPosition = transform.localPosition;
        updateTime += Random.Range(0.0f, 0.1f);
        // CorrectPosition();
    }

    public void CorrectPosition() {
        transform.localPosition = correctedPosition;
        SendCustomEventDelayedSeconds("CorrectPosition", 0.5f);
    }


    public void Update()
    {
        if (fishingPole.localPlayerClose) {
            transform.localPosition = correctedPosition;
        }
    }
}
