
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SunSpinnerHandle : UdonSharpBehaviour
{
    public SunSpinner spinner;
    public Transform handleTarget;
    public VRCPickup pickup;

    public override void OnPickup()
    {
        if (Networking.GetOwner(gameObject).isLocal) {
            Networking.SetOwner(Networking.LocalPlayer, spinner.gameObject);
            Networking.SetOwner(Networking.LocalPlayer, spinner.cycle.gameObject);
        }
    }

    public override void OnDrop()
    {
        if (Networking.GetOwner(gameObject).isLocal) {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetPosition));
            spinner.cycle.FinalizeAngle(spinner.transform.eulerAngles.y);
        }
    }

    public void ResetPosition() {
        Debug.LogFormat("{0}: ResetPosition", name);
        transform.SetLocalPositionAndRotation(handleTarget.localPosition, handleTarget.localRotation);
        SendCustomEventDelayedSeconds(nameof(DelayResetPosition), 1f);
    }

    public void DelayResetPosition() {
        transform.SetLocalPositionAndRotation(handleTarget.localPosition, handleTarget.localRotation);
    }
}
