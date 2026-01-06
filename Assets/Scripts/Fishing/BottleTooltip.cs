using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class BottleTooltip : UdonSharpBehaviour
{
    public Text bottleNameText;
    public RawImage icon;

    
    public float goalScale = 0.001f;
    public float scaleSpeed = 4f;
    bool isActive = false;
    bool isDeactivating = false;
    bool isScaling = false;
    public float activeTime = 2f;
    public float activeTimer = 0f;


    public void UpdateTooltip(Bottle bottle)
    {
        SetName(bottle);
        SetIcon(bottle);
    }

    public void SetName(Bottle bottle)
    {
        if (bottle.fillLevel > 0 && bottle.liquid != null)
        {
            bottleNameText.text = bottle.liquid.name;
        }
        else
        {
            bottleNameText.text = "Empty Bottle";
        }
    }

    public void SetIcon(Bottle bottle)
    {
        if (bottle.fillLevel > 0)
        {
            if (bottle.liquid == null) {
                icon.enabled = false;
                return;
            }
            if (bottle.liquid.name == "Fish Oil") {
            }
            else if (bottle.liquid.name == "Flamefin Tears") {
            }
            else if (bottle.liquid.name == "Essence of Water") {
            }
            else if (bottle.liquid.name == "Boiled Bladder") {
            }
            else if (bottle.liquid.name == "Digestive Mud") {
            }
            else if (bottle.liquid.name == "Heart of Trout") {
            }
            else if (bottle.liquid.name == "Bioluminescent Bile") {
            }
            else if (bottle.liquid.name == "Distilled Darkness") {
            }
            else if (bottle.liquid.name == "Swiftfin Slime") {
            }
            else if (bottle.liquid.name == "Ocular Juice") {
            }
            else if (bottle.liquid.name == "Batfish Guano") {
            }
            else if (bottle.liquid.name == "Piranha Milk") {
            }
            else if (bottle.liquid.name == "Stinky Mucus") {
            }
            else if (bottle.liquid.name == "Mishmash") {
            }
            else if (bottle.liquid.name == "Silvered Silt") {
            }
            else if (bottle.liquid.name == "Golden Gumbo") {
            }
            else {
                icon.enabled = false;
                return;
            }
            icon.uvRect = new Rect(bottle.liquid.UVOffsetX, bottle.liquid.UVOffsetY, .25f, .25f); // Reset UV rect to full texture
            icon.color = bottle.liquid.color;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
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
