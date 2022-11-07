
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Basket : UdonSharpBehaviour
{
    public bool dump = false;
    private bool dumping = false;

    private void Update()
    {
        if (dump) DumpContents();
        else if (!dump && dumping)
        {
            dumping = false;
        }
    }

    void DumpContents()
    {
        dumping = true;
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(true);
            child.SetParent(null);
        }
    }

    void AddToBasket(GameObject fish)
    {
        fish.transform.SetParent(transform);
        fish.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.LogFormat("{0}: {1} entered trigger from layer {2}", name, other.gameObject.name, other.gameObject.layer);
        if (!dumping && other.gameObject.layer == 24)
        {
            AddToBasket(other.gameObject);
        }
    }
}
