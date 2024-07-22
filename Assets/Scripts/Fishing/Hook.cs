
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Hook : UdonSharpBehaviour
{
    public FishForce fishForce;
    public MeshFilter baitMesh;
    public MeshRenderer meshRenderer;

    public void AddBait(Bait bait) {
        Debug.LogFormat("{0}: Adding Bait {1}", name, bait.name);
        fishForce.AddBait(bait);
        baitMesh.sharedMesh = bait.mesh;
        meshRenderer.material.color = bait.material.color;
    }

    public void RemoveBait() {
        Debug.LogFormat("{0}: Removing Bait", name);
        baitMesh.sharedMesh = null;
    }

    public void OnTriggerEnter(Collider other) {
        if (Networking.GetOwner(gameObject).isLocal) {
            // Debug.LogFormat("{0}: Trigger Entered: {1}", name, other.name);
            BaitItem bait = other.GetComponent<BaitItem>();
            if (bait != null) {
                AddBait(bait.bait);
                bait.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "RemoveBait");
            }
        }
    }
}
