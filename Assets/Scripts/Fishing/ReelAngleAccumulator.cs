
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class ReelAngleAccumulator : UdonSharpBehaviour
{

    public FishingPole fishingPole = null;
    public Text angleText = null;

    public Transform handleTarget;
    public HandleHandler handleTargetHandler;

    public Transform handle;

    private float angle;
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

    void Start()
    {
        previousRotation = transform.localRotation;
        previousVector = handleTarget.localPosition - transform.localPosition;
        previousVector.x = 0f;
        previousVector = previousVector.normalized;
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

                }
            }
            fishingLine.localPosition = pos;
        }
    }
}
