
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fireflies : UdonSharpBehaviour
{
    public Animator animator;
    public Material material;
    public NewSuperDayNightCycle dayNightCycle;

    [ColorUsageAttribute(true,true)]
    public Color lakeColor = Color.white;
    [ColorUsageAttribute(true,true)]
    public Color caveColor = Color.white;
    [ColorUsageAttribute(true,true)]
    public Color mudColor = Color.white;
    [ColorUsageAttribute(true,true)]
    public Color iceColor = Color.white;


    public void Start()
    {
        material.SetColor("_Color", lakeColor);
        material.SetColor("_EmissionColor", lakeColor);
    }

    public void Update()
    {
        animator.SetFloat("Emission", Mathf.Clamp(dayNightCycle.dayNightValue, 0f, 1f));
    }

    public void SetColor(int type)
    {
        Color color = Color.white;
        switch (type)
        {
            case 0:
                color = lakeColor;
                break;
            case 1:
                color = caveColor;
                break;
            case 2:
                color = mudColor;
                break;
            case 3:
                color = iceColor;
                break;
        }
        material.SetColor("_Color", color);
        material.SetColor("_EmissionColor", color);
    }
}
