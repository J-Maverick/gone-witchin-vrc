using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PourableBottle : Bottle
{
    [Range(0,1)]
    public float pourThreshold = 0.5f;
    public float maxPourThreshold = 0.5f;
    public float minPourThreshold = -0.2f;
    [UdonSynced] public float fillLevel = 0f;
    public Animator pourAnimator;
    public float pourMultiplier = 0.01f;

    public Renderer particleRenderer;
    public Material particleMaterial;
    public float pourSpeed = 0f;

    protected override void Start()
    {
        particleMaterial = particleRenderer.material;
        particleMaterial.color = potionColor;
        base.Start();
    }

    public override void OnDeserialization()
    {
        if (shaderControl != null) shaderControl.fillLevel = fillLevel;
    }

    bool CheckPour()
    {
        return transform.up.y < pourThreshold;
    }

    float GetPourSpeed()
    {
        return (pourThreshold - transform.up.y) / (pourThreshold + 1f);
    }

    private void UpdateFill()
    {
        pourThreshold = maxPourThreshold - ((1f - fillLevel) * (maxPourThreshold - minPourThreshold));
        pourSpeed = GetPourSpeed();
        fillLevel -= pourSpeed * pourMultiplier * Time.deltaTime;
        if (fillLevel < 0f) fillLevel = 0f;
        if (shaderControl != null) shaderControl.fillLevel = fillLevel;
    }

    public void SetFill(float fill)
    {
        fillLevel = fill;
        pourThreshold = maxPourThreshold - ((1f - fillLevel) * (maxPourThreshold - minPourThreshold));
        if (shaderControl != null) shaderControl.fillLevel = fillLevel;
    }

    private void Update()
    {
        if (fillLevel > 0f && CheckPour())
        {
            UpdateFill();
            //Debug.LogFormat("{0}: transform.up.y: {1}, pourSpeed: {2}", name, transform.up.y, pourSpeed);
        }
        else pourSpeed = 0f;

        pourAnimator.SetFloat("pourSpeed", pourSpeed);
    }
}
