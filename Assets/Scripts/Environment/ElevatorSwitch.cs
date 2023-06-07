
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ElevatorSwitch : UdonSharpBehaviour
{
    public Elevator elevator;
    public Animator animator;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            elevator.Move();
        }
    }

    public void SetOn()
    {
        animator.SetBool("Moving", true);
    }

    public void SetOff()
    {
        animator.SetBool("Moving", false);
    }
}
