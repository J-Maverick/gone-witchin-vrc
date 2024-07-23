
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Elevator : UdonSharpBehaviour
{
    public Transform startLocation;
    public Transform endLocation;
    public Vector3 previousPosition;
    // public Vector3 previousVelocity;
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
            // previousVelocity = Vector3.zero;
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
            previousPosition = transform.position;
            // previousVelocity = Vector3.zero;
        }
    }

    public void Update()
    {
        if (moveActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation.position, moveSpeed * Time.deltaTime);
            // Vector3 velocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
            if (transform.position == targetLocation.position)
            {
                moveActive = false;
                elevatorSwitch.SetOff();
            }
            if (playerColliding) {
                // Networking.LocalPlayer.SetVelocity(velocity);
                Networking.LocalPlayer.TeleportTo(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin).position + transform.position - previousPosition, Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin).rotation, VRC_SceneDescriptor.SpawnOrientation.AlignRoomWithSpawnPoint, true);
            }
            previousPosition = transform.position;
            // previousVelocity = velocity;
        }
    }
}
