
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SunSpinner : UdonSharpBehaviour
{
    public DayNightCycle cycle;
    public SunSpinnerHandle handle;
    public float maxRotationSpeed = 1f;
    [UdonSynced] public bool spinnerActive = false;

    public void Activate() {
        spinnerActive = true;
        RequestSerialization();
    }

    public void LateUpdate() {
        if (!spinnerActive) return;
        if (!handle.pickup.IsHeld) {
            transform.eulerAngles = new Vector3(0, cycle.angle, 0);
        }
        else {
            if (Networking.GetOwner(gameObject).isLocal) {
                Vector3 targetVector = handle.transform.localPosition;
                targetVector.y = 0f;
                targetVector = targetVector.normalized;

                Vector3 currentVector = handle.handleTarget.localPosition;
                currentVector.y = 0f;
                currentVector = currentVector.normalized;

                float angle = Vector3.SignedAngle(currentVector, targetVector, Vector3.up);

                transform.RotateAround(transform.position, Vector3.up, Mathf.Clamp(angle, 0f, maxRotationSpeed * Time.deltaTime));

                cycle.SetAngle(transform.eulerAngles.y);
            }

        }
    }
}
