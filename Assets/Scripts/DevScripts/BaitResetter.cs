
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class BaitResetter : UdonSharpBehaviour
{
    public BaitInventory baitInventory;

    [ContextMenu("Reset Baits")]
    public void ResetBaits()
    {
        foreach (Transform child in transform)
        {
            CollectibleBait collBait = child.GetComponent<CollectibleBait>();
            collBait.inventory = baitInventory;
            collBait.baitCollider.enabled = false;
            if (collBait.baitLight != null)
            {
                collBait.baitLight.enabled = false;
            }
            if (collBait.baitRenderer != null)
            {
                collBait.baitRenderer.enabled = false;
            }
            if (collBait.baitRenderers != null)
            {
                foreach (Renderer rend in collBait.baitRenderers)
                {
                    rend.enabled = false;
                }
            }
            foreach (Bait bait in baitInventory.baits)
            {
                if (collBait.name.Contains(bait.name))
                {
                    collBait.bait = bait;
                    break;
                }
            }
        }
    }

    [ContextMenu("Show Baits")]
    public void ShowBaits()
    {
        foreach (Transform child in transform)
        {
            CollectibleBait collBait = child.GetComponent<CollectibleBait>();
            collBait.baitCollider.enabled = true;
            if (collBait.baitLight != null)
            {
                collBait.baitLight.enabled = true;
            }
            if (collBait.baitRenderer != null)
            {
                collBait.baitRenderer.enabled = true;
            }
            if (collBait.baitRenderers != null)
            {
                foreach (Renderer rend in collBait.baitRenderers)
                {
                    rend.enabled = true;
                }
            }
        }
    }
}
