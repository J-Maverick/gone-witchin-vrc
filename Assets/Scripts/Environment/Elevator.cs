
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Elevator : UdonSharpBehaviour
{
    public Transform startLocation;
    public Transform endLocation;
    private Transform targetLocation;
    public float moveSpeed = .5f;

    private bool moveActive = false;
    public ElevatorSwitch elevatorSwitch;

    public bool playerColliding = false;

    public PlayerStatBooster statBooster;

    public void Start()
    {
        targetLocation = startLocation;
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            playerColliding = true;

        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            playerColliding = false;
        }
    }

    public void Move()
    {
        if (!moveActive)
        {
            moveActive = true;
            elevatorSwitch.SetOn();
            targetLocation = targetLocation == startLocation ? endLocation : startLocation;
        }
    }

    public void Update()
    {
        if (moveActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation.position, moveSpeed * Time.deltaTime);
            if (transform.position == targetLocation.position)
            {
                moveActive = false;
                elevatorSwitch.SetOff();
            }
        }
    }
}
