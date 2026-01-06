
using UdonSharp;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using VRC.SDKBase;
using VRC.Udon;

public class ElevatorSwitch : UdonSharpBehaviour
{
    public Elevator elevator;
    public Animator animator;

    public bool switchEnabled = true;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal && switchEnabled)
        {
            elevator.Move();
            switchEnabled = false;
        }
    }

    public virtual void SetOn()
    {
        animator.SetBool("Moving", true);
    }

    public virtual void SetOff()
    {
        animator.SetBool("Moving", false);
        SendCustomEventDelayedSeconds(nameof(EnableSwitch), 0.25f);
    }

    public virtual void EnableSwitch() {
        switchEnabled = true;
    }
}
