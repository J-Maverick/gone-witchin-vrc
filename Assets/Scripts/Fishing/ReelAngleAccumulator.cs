
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

    

    void Start()
    {
        previousRotation = transform.localRotation;
        previousVector = handleTarget.localPosition - transform.localPosition;
        previousVector.x = 0f;
        previousVector = previousVector.normalized;
    }

    private void Update()
    {
        if (handleTargetHandler.dropped)
        {
            handleTarget.SetPositionAndRotation(handle.position, handle.rotation);
            previousVector = handleTarget.localPosition - transform.localPosition;
            previousVector.x = 0f;
            previousVector = previousVector.normalized;
            handleTargetHandler.dropped = false;
        }
        Vector3 targetVector = handleTarget.localPosition - transform.localPosition;
        targetVector.x = 0f;
        targetVector = targetVector.normalized;
        transform.localRotation = Quaternion.FromToRotation(previousVector, targetVector) * transform.localRotation;
        previousVector = targetVector;
        if (transform.localRotation != previousRotation)
        {
            Quaternion fromPrevToCurrent = Quaternion.Inverse(previousRotation) * transform.localRotation;

            float delta = fromPrevToCurrent.eulerAngles.y;
            if (delta > 180) delta -= 360;
            angle -= delta;

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
    }
}
