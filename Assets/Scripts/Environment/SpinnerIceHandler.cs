
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SpinnerIceHandler : UdonSharpBehaviour
{
    public SunSpinner sunSpinner;
    public NewSuperDayNightCycle dayNightCycle;
    public Vector3 targetScale;
    public DestructibleObject rockDestructible;
    public DestructibleObject iceDestructible;
    public Transform iceTransform;
    [UdonSynced] public bool frozen = false;
    public bool freezingComplete = false;

    public void Start()
    {
        targetScale = iceTransform.localScale;
    }

    // Called by Ice Destructible Object when destroyed
    public void Activate()
    {
        sunSpinner.Activate();
        dayNightCycle.Unfreeze();
        frozen = false;
        RequestSerialization();
    }

    // Called by Ice potion
    public void FreezeSpinner()
    {
        if (frozen) return;
        if (!rockDestructible.Destroyed) return;

        frozen = true;
        sunSpinner.DeActivate();
        dayNightCycle.Freeze();

        iceTransform.localScale = Vector3.zero;
        iceDestructible.Reset();
        freezingComplete = false;
    }

    public void Update()
    {
        if (!frozen) return;
        if (freezingComplete) return;
        iceTransform.localScale = Vector3.Lerp(iceTransform.localScale, targetScale, Time.deltaTime);

        if (Vector3.Distance(iceTransform.localScale, targetScale) < 0.001f)
        {
            iceTransform.localScale = targetScale;
            freezingComplete = true;
        }
    }
}
