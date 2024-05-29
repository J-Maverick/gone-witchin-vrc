
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class WheelAngleAccumulator : UdonSharpBehaviour
{

    public RecipeBook recipeBook;

    public Transform handleTarget;
    public HandleHandler handleTargetHandler;

    public Transform handle;

    public float pageTurnAngle = 30f;
    private float angle = 0f;
    private float prevAngle = 0f;
    private Quaternion previousRotation;
    private Vector3 previousVector;

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
        prevAngle = angle;
    }

    public void PickUp() {
        prevAngle = angle;
    }

    private void Update()
    {

        if (handleTargetHandler.isHeld)
        {
            Vector3 targetVector = handleTarget.localPosition - transform.localPosition;
            targetVector.x = 0f;
            targetVector = targetVector.normalized;
            transform.localRotation = Quaternion.FromToRotation(previousVector, targetVector) * transform.localRotation;
            previousVector = targetVector;

            
            if (transform.localRotation != previousRotation)
            {
                Quaternion fromPrevToCurrent = Quaternion.Inverse(previousRotation) * transform.localRotation;

                float delta = fromPrevToCurrent.eulerAngles.x;
                if (delta > 180) delta -= 360;
                angle -= delta;

                previousRotation = transform.localRotation;

                float angleDiff = angle - prevAngle;

                if (angleDiff >= pageTurnAngle) {
                    recipeBook.NextRecipe();
                    prevAngle = angle;
                }
                else if (angleDiff <= -pageTurnAngle) {
                    recipeBook.PreviousRecipe();
                    prevAngle = angle;
                }
            }
        }
    }
}
