
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class Lever : UdonSharpBehaviour
{
    public Transform handleTarget;
    public HandleHandler handleTargetHandler;

    public Transform handle;

    public float angle;
    public Transform maxPull;
    public Transform minPull;

    private Quaternion previousRotation;
    private Vector3 previousVector;
    private Quaternion defaultRotation;

    private Vector3 targetVector;
    private Quaternion targetRotation;
    private Vector3 defaultTargetLocation;
    private Quaternion maxRotation;
    private Quaternion minRotation;
    public float maxAngle;
    public float minAngle;
    public float absoluteAngle;
    public VRC_Pickup pickup;

    void Start()
    {
        defaultRotation = transform.localRotation;
        defaultTargetLocation = handleTarget.position;
        previousRotation = transform.localRotation;
        previousVector = handleTarget.position - transform.position;
        previousVector.x = 0f;
        previousVector = previousVector.normalized;
    }

    private void FixedUpdate()
    {
        if (handleTargetHandler.dropped)
        {
            handleTarget.SetPositionAndRotation(handle.position, handle.rotation);
            previousVector = handleTarget.position - transform.position;
            previousVector.x = 0f;
            previousVector = previousVector.normalized;
            handleTargetHandler.dropped = false;
        }


        targetVector = handleTarget.position - transform.position;
        targetVector.x = 0f;
        targetVector = targetVector.normalized;

        targetRotation = Quaternion.FromToRotation(previousVector, targetVector) * transform.localRotation;


        previousVector = targetVector;
        float delta = 0f;
        if (targetRotation != previousRotation)
        {
            Quaternion fromPrevToCurrent = Quaternion.Inverse(previousRotation) * targetRotation;

            delta = fromPrevToCurrent.eulerAngles.y;
            if (delta > 180) delta -= 360;
            angle -= delta;

            if (pickup.IsHeld)
            {
                if (angle > maxAngle)
                {
                    targetRotation = Quaternion.Euler(-maxAngle, 0f, -90f);
                    angle = maxAngle;
                }
                else
                if (angle < minAngle)
                {
                    targetRotation = Quaternion.Euler(minAngle, 0f, -90f);
                    angle = minAngle;
                }
            }
            else if (angle < 0f) angle = 0f;

            transform.localRotation = targetRotation;
            previousRotation = transform.localRotation;
        }


        if (!pickup.IsHeld && transform.localRotation != defaultRotation)
        {
            float tempDelta = (Quaternion.Inverse(defaultRotation) * transform.localRotation).eulerAngles.y;
            if (Mathf.Abs(tempDelta) < 0.5f)
            {
                transform.localRotation = defaultRotation;
                previousRotation = transform.localRotation;
                targetVector = handleTarget.position - transform.position;
                targetVector.x = 0f;
                targetVector = targetVector.normalized;
                previousVector = targetVector;
                angle = 0f;
                Debug.LogFormat("{0}: Snapped.", name);
            }
            else
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, defaultRotation, 1f);
            }
        }

        if (!pickup.IsHeld)
            if (angle > 0.5f) angle /= 1.5f;
            else angle = 0f;
    }
}
