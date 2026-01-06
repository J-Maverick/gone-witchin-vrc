
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.SDK3.Components;

public class ReelAngleAccumulator : UdonSharpBehaviour
{

    public FishingPole fishingPole = null;
    public Text angleText = null;

    public Transform handleTarget;
    public HandleHandler handleTargetHandler;
    public Vector3 startPosition;

    public Transform handle;
    public VRCPickup pickup;

    public float angle;
    private Quaternion previousRotation;
    private Vector3 previousVector;

    public Transform fishingLine;
    public SkinnedMeshRenderer fishingPoleRenderer;
    public float sinAmplitude = 0.015f;

    public float castDistance = 0f;
    public float maxCastDistance = 60f;
    public float maxBlendShapeValue = 59f;
    public float blendShapeValue = 0f;
    private float lineYMin = 0.15933f;
    private float lineZMin = -0.17681f;
    private float lineYMax = 0.1497f;
    private float lineZMax = -0.1505f;

    private float prevTick = 0f;
    private float tickAngle = 25f;

    public float desktopReelAmount = 0.1f;

    public AudioSource reelSound;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float maxDelta = 0.1f;
    public float minDelta = 0.01f;
    public float pitchDeltaMultiplier = 1f;

    void Start()
    {
        previousRotation = transform.localRotation;
        startPosition = transform.localPosition;
        previousVector = handleTarget.localPosition - transform.localPosition;
        previousVector.x = 0f;
        previousVector = previousVector.normalized;
        reelSound.Play();
        reelSound.Pause();
    }

    public void Drop() {
        handleTarget.SetPositionAndRotation(handle.position, handle.rotation);
        previousVector = handleTarget.localPosition - transform.localPosition;
        previousVector.x = 0f;
        previousVector = previousVector.normalized;
        handleTargetHandler.dropped = false;
    }

    private void Update()
    {
        if (fishingPole.isHeld)
        {
            float fishingPoleDistance = fishingPole.GetCastDistance();
            if (fishingPole.lureJoint != null)
            {
                if (fishingPoleDistance != castDistance)
                {
                    castDistance = fishingPoleDistance;
                    blendShapeValue = 100f * castDistance / maxCastDistance;
                }
                else if (castDistance == 0f) blendShapeValue = 0f;
                else blendShapeValue = 100f * (fishingPole.lureJoint.minDistance / castDistance) * (castDistance / maxCastDistance);
            }
            if (blendShapeValue > maxBlendShapeValue) blendShapeValue = maxBlendShapeValue;
            fishingPoleRenderer.SetBlendShapeWeight(0, blendShapeValue);
        }

        if (handleTargetHandler.isHeld)
        {
            Vector3 targetVector = handleTarget.localPosition - transform.localPosition;
            targetVector.x = 0f;
            targetVector = targetVector.normalized;
            transform.localRotation = Quaternion.FromToRotation(previousVector, targetVector) * transform.localRotation;
            previousVector = targetVector;

            Vector3 pos = fishingLine.localPosition;

            pos.y = lineYMin + ((blendShapeValue / 59f) * (lineYMax - lineYMin));
            pos.z = lineZMin + ((blendShapeValue / 59f) * (lineZMax - lineZMin));
            if (transform.localRotation != previousRotation)
            {
                Quaternion fromPrevToCurrent = Quaternion.Inverse(previousRotation) * transform.localRotation;

                float delta = fromPrevToCurrent.eulerAngles.x;
                if (delta > 180) delta -= 360;
                angle -= delta;

                pos.x = Mathf.Sin(angle / 1000f) * sinAmplitude;

                previousRotation = transform.localRotation;

                if (angleText != null)
                {
                    angleText.text = string.Format("Angle: {0:0.##}", angle);
                }
                if (fishingPole != null)
                {
                    if (delta < 0f) fishingPole.AddSpring(-delta);
                    if (Mathf.Abs(angle - prevTick) > tickAngle) {
                        fishingPole.TickVibration();
                        prevTick = angle;
                    }
                }
                if (delta < 0f) {
                    float pitchDelta = pitchDeltaMultiplier * Mathf.Abs(delta) / (Time.deltaTime * 180f);
                    if (pitchDelta < minDelta) pitchDelta = minDelta;
                    if (pitchDelta > maxDelta) pitchDelta = maxDelta;
                    reelSound.pitch = Mathf.Lerp(minPitch, maxPitch, pitchDelta);
                    if (!reelSound.isPlaying) {
                        reelSound.UnPause();
                    }
                }
                else if (reelSound.isPlaying) {
                    reelSound.Pause();
                }
            }
            else {
                if (reelSound.isPlaying) {
                    reelSound.Pause();
                }
            }
            fishingLine.localPosition = pos;
        }
        else if (fishingPole.desktopReeling) { 
            transform.RotateAround(transform.position, transform.right, -desktopReelAmount * Time.deltaTime);
            transform.localPosition = startPosition;
            handleTarget.SetPositionAndRotation(handle.position, handle.rotation);

            Vector3 pos = fishingLine.localPosition;

            pos.y = lineYMin + ((blendShapeValue / 59f) * (lineYMax - lineYMin));
            pos.z = lineZMin + ((blendShapeValue / 59f) * (lineZMax - lineZMin));
            if (transform.localRotation != previousRotation)
            {
                Quaternion fromPrevToCurrent = Quaternion.Inverse(previousRotation) * transform.localRotation;

                float delta = fromPrevToCurrent.eulerAngles.x;
                if (delta > 180) delta -= 360;
                angle -= delta;

                pos.x = Mathf.Sin(angle / 1000f) * sinAmplitude;

                previousRotation = transform.localRotation;

                if (angleText != null)
                {
                    angleText.text = string.Format("Angle: {0:0.##}", angle);
                }
                if (fishingPole != null)
                {
                    if (delta < 0f) fishingPole.AddSpring(-delta);
                    if (Mathf.Abs(angle - prevTick) > tickAngle) {
                        prevTick = angle;
                    }
                }
                if (delta < 0f) {
                    reelSound.pitch = 1f;
                    if (!reelSound.isPlaying) {
                        reelSound.UnPause();
                    }
                }
                else if (reelSound.isPlaying) {
                    reelSound.Pause();
                }
            }
            else {
                if (reelSound.isPlaying) {
                    reelSound.Pause();
                }
            }
            fishingLine.localPosition = pos;
        }
        else {
            if (reelSound.isPlaying) {
                reelSound.Pause();
            }
        }
    }
}
