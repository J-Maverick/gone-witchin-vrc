﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RecipePanel : UdonSharpBehaviour
{
    public Renderer rend;
    public Material material;

    public void Start() {
        material = rend.materials[0];
    }

    public void ResetPanel() {
        material.SetColor("_EmissionColor", Color.black);
    }

    public void SetPanel(LiquidMaterial liquid, bool wrap=false) {
        material.SetTextureOffset("_MainTex", new Vector2(liquid.UVOffsetX, liquid.UVOffsetY));
        if (wrap) {
            material.SetColor("_EmissionColor", Color.Lerp(liquid.color, Color.black, 0.5f));
        }
        else {
            material.SetColor("_EmissionColor", liquid.color);
        }
    }
}
