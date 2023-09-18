
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BGMToggle : UdonSharpBehaviour
{

    public GameObject musicObject;

    public override void Interact()
    {
        musicObject.SetActive(!musicObject.activeSelf);
    }
}
