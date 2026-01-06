
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ElevatorSwitchButton : ElevatorSwitch
{
    public Collider buttonCollider;
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
    }

    public override void Interact() {
        if (switchEnabled)
        {
            elevator.Move();
            switchEnabled = false;
        }
    }

    public override void SetOn()
    {
        buttonCollider.enabled = false;
    }

    public override void SetOff()
    {
        SendCustomEventDelayedSeconds(nameof(EnableSwitch), 0.25f);
    }

    public override void EnableSwitch() {
        switchEnabled = true;
        buttonCollider.enabled = true;
    }
}
