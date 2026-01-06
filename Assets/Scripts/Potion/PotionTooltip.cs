
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class PotionTooltip : UdonSharpBehaviour
{
    public Text bottleNameText;
    public Text bottleDescriptionText;
    
    public float goalScale = 0.001f;
    public float scaleSpeed = 4f;
    bool isActive = false;
    bool isDeactivating = false;
    bool isScaling = false;
    public float activeTime = 5f;
    public float activeTimer = 0f;


    public void UpdateTooltip(ThrowablePotionBottle bottle)
    {
        SetName(bottle);
    }

    public void SetName(ThrowablePotionBottle bottle)
    {
        bottleNameText.text = bottle.liquid.name;
        bottleDescriptionText.text = bottle.liquid.description;
    }

    public void Activate()
    {
        isActive = true;
        isDeactivating = false;
        isScaling = true;
        transform.localScale = Vector3.zero;
        activeTimer = 0f;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        if (!isActive) return;
        isDeactivating = true;
        isScaling = true;
    }

    private void Update()
    {
        if (isActive)
        {
            if (isScaling)
            {
                if (isDeactivating)
                {
                    transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, (goalScale * scaleSpeed) * Time.deltaTime);
                    if (transform.localScale.magnitude < 0.001f)
                    {
                        isActive = false;
                        isDeactivating = false;
                        isScaling = false;
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(goalScale, goalScale, goalScale), (goalScale * scaleSpeed) * Time.deltaTime);
                    if (transform.localScale.x == goalScale)
                    {
                        isScaling = false;
                    }
                }
            }
            // Ensure the tooltip is always facing the player's head
            // transform.LookAt(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position);
            transform.rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            // transform.Rotate(0, 180, 0); // Adjust rotation to face the player correctly
            // Update the active timer
            activeTimer += Time.deltaTime;
            if (activeTimer >= activeTime)
            {
                Deactivate();
            }
        }
    }
}
