
using System.Collections.Specialized;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PersonalInventory : UdonSharpBehaviour
{
    public GameObject inventory;
    public PipeSpawner pipeSpawner;
    public bool localPlayer = false;
    public bool inventoryState = false;
    public float inventoryDistanceFactor = 1.0f;
    public float inventoryScaleFactor = 1f;
    public float currentScaleFactor = 1f;
    public float eyeHeightFactor = 1f;
    public float transitionSpeed = 2f;

    public float startDistance = 0f;
    public bool holdingTriggers = false;
    public bool opening = false;
    public bool closing = false;

    void Update()
    {
        if (localPlayer && Input.GetKeyDown("p"))
        {
            ToggleInventory();
            opening = inventoryState;
            closing = !inventoryState;
        }
        if (localPlayer && Input.GetButton("Oculus_CrossPlatform_PrimaryIndexTrigger") && Input.GetButton("Oculus_CrossPlatform_SecondaryIndexTrigger"))
        {
            float distance = Vector3.Distance(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position, Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position);
            if (!holdingTriggers)
            {
                startDistance = distance;
            }
            if (distance > startDistance * 2f)
            {
                inventoryState = true;
                inventory.gameObject.SetActive(inventoryState);
                if (!opening)
                {
                    SpawnInventory();
                }
                opening = true;
                closing = false;
            }
            else if (distance < startDistance / 2f)
            {
                inventoryState = false;
                pipeSpawner.pickup.Drop();
                closing = true;
                opening = false;
            }
            holdingTriggers = true;
        }
        else
        {
            holdingTriggers = false;
        }
        if (opening)
        {
            currentScaleFactor = Mathf.MoveTowards(currentScaleFactor, inventoryScaleFactor, Time.deltaTime * transitionSpeed);
            if (currentScaleFactor >= inventoryScaleFactor)
            {
                currentScaleFactor = inventoryScaleFactor;
                if (!holdingTriggers) opening = false;
            }
            float eyeHeight = Networking.LocalPlayer.GetAvatarEyeHeightAsMeters();
            inventory.transform.localScale = Vector3.one * eyeHeight * currentScaleFactor;
        }
        else if (closing)
        {
            currentScaleFactor = Mathf.MoveTowards(currentScaleFactor, 0f, Time.deltaTime * transitionSpeed);
            float eyeHeight = Networking.LocalPlayer.GetAvatarEyeHeightAsMeters();
            if (currentScaleFactor < 0.01f)
            {
                currentScaleFactor = 0f;
                inventory.gameObject.SetActive(false);
                closing = false;
            }
            inventory.transform.localScale = Vector3.one * eyeHeight * currentScaleFactor;
        }
    }

    public void ToggleInventory()
    {
        inventoryState = !inventoryState;
        SpawnInventory();
        if (!inventoryState)
        {
            pipeSpawner.pickup.Drop();
        }
        else
        {
            inventory.gameObject.SetActive(inventoryState);
        }
    }

    public void SpawnInventory() {
        if (inventoryState) {
            Debug.Log("Spawning inventory");
            currentScaleFactor = 0f;
            float eyeHeight = Networking.LocalPlayer.GetAvatarEyeHeightAsMeters();
            inventory.transform.position = Networking.LocalPlayer.GetPosition() + Networking.LocalPlayer.GetRotation() * Vector3.forward * eyeHeight / inventoryDistanceFactor + new Vector3(0, eyeHeight * eyeHeightFactor, 0);
            inventory.transform.rotation = Networking.LocalPlayer.GetRotation();
            inventory.transform.Rotate(0, 180, 0);
        }
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (Networking.GetOwner(gameObject).isLocal) {
            localPlayer = true;
        }
        // if (Networking.GetOwner(gameObject) == player) {
        //     baitInventory.baitInventory.AddEndpoint(baitInventory);
        // }
    }

}
