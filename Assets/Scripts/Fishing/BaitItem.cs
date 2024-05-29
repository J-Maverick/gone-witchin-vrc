
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class BaitItem : UdonSharpBehaviour
{
    public Bait bait;
    public VRCObjectPool baitPool;
    public VRCObjectSync baitSync;

    public void OnEnable() {
        baitSync.FlagDiscontinuity();
    }

    public void RemoveBait() {
        baitPool.Return(gameObject);
    }
    
}
