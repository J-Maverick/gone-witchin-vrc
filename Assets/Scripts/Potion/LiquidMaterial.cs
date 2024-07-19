
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LiquidMaterial : UdonSharpBehaviour
{
    public int ID;
    public Color color;
    public float UVOffsetX = 0f;
    public float UVOffsetY = 0f;
    public Bait bait = null;
}
