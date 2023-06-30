
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TriggerLever : Lever
{
    public bool triggered = false;

    public virtual void Trigger() {}

    public override void FixedUpdate() {
        base.FixedUpdate();

        if (!isSleeping) {
            if (!triggered) {
                if (angle == maxAngle) {
                    Trigger();
                    triggered = true;
                }
            }
            else if (angle == minAngle) {
                triggered = false;
            }
        }
    }
}
