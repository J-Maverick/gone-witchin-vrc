
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PersonalFishPipe : UdonSharpBehaviour
{
    public Material material;
    public MeshRenderer meshRenderer;

    public void Start()
    {
        material = meshRenderer.materials[0];
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        Random.InitState(Networking.GetOwner(gameObject).displayName.GetHashCode());
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        material.SetColor("_Color", randomColor);
        Random.InitState(Time.frameCount);
    }

    public void Spawn() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(DisableGameObject));
        SendCustomEventDelayedSeconds(nameof(DelayEnable), 0.25f);
    }

    public void DisableGameObject() {
        gameObject.SetActive(false);
    }

    public void DelayEnable() {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(EnableGameObject));
    }

    public void EnableGameObject()
    {
        gameObject.SetActive(true);
    }
}
