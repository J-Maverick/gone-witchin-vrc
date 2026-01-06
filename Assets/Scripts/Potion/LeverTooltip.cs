
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class LeverTooltip : UdonSharpBehaviour
{
    
    public Text bottleNameText;
    public RawImage icon;

    
    public float goalScale = 0.001f;
    public float scaleSpeed = 4f;
    bool isActive = false;
    bool isDeactivating = false;
    bool isScaling = false;
    public float activeTime = 3f;
    public float activeTimer = 0f;


    public void UpdateTooltip(ReagentTank tank)
    {
        SetName(tank);
        SetIcon(tank);
    }

    public void SetName(ReagentTank tank)
    {
        bottleNameText.text = tank.reagent.name;
    }

    public void SetIcon(ReagentTank tank)
    {
        icon.uvRect = new Rect(tank.reagent.UVOffsetX, tank.reagent.UVOffsetY, .25f, .25f); // Reset UV rect to full texture
        icon.color = tank.reagent.color;
        icon.enabled = true;
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
            // transform.Rotate(0, 180, 0); // Adjust rotation to face the player correctly
            
            transform.rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            // Update the active timer
            activeTimer += Time.deltaTime;
            if (activeTimer >= activeTime)
            {
                Deactivate();
            }
        }
    }
}
