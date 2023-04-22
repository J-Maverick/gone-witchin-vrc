
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GemIndicator : UdonSharpBehaviour
{
    public Renderer rend;
    public int emissionMultiplier = 4;
    private Material material;
    private Color invalidColor = Color.red;
    private Color validColor = Color.green;
    private bool isValid = false;

    private void Start()
    {
        material = rend.materials[0];
        SetInvalid();
    }

    public void SetInvalid()
    {
        material.color = invalidColor;
        material.SetColor("_EmissionColor", invalidColor * emissionMultiplier);
        isValid = false;
    }

    public void SetValid()
    {
        material.color = validColor;
        material.SetColor("_EmissionColor", validColor * emissionMultiplier);
        isValid = true;
    }

    public bool IsValid()
    {
        return isValid;
    }
}
