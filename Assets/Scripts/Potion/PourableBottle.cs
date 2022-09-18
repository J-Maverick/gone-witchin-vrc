using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PourableBottle : Bottle
{
    [Range(0,1)]
    public float pourThreshold = 0.6f;
    public float fillLevel = 0f;
    public Animator pourAnimator;
    public float pourMultiplier = 0.01f;

    private float pourSpeed = 0f;

    protected override void Start()
    {
        base.Start(); 
    }

    bool CheckPour()
    {
        return transform.up.y < pourThreshold;
    }

    float GetPourSpeed()
    {
        return (pourThreshold - transform.up.y) / (pourThreshold + 1f);
    }

    private void Update()
    {
        if (fillLevel > 0f && CheckPour())
        {
            pourSpeed = GetPourSpeed();
            fillLevel -= pourSpeed * pourMultiplier * Time.deltaTime;
            if (fillLevel < 0f) fillLevel = 0f;

            if (shaderControl != null) shaderControl.fillLevel = fillLevel;
            Debug.LogFormat("{0}: transform.up.y: {1}, pourSpeed: {2}", name, transform.up.y, pourSpeed);
        }
        else pourSpeed = 0f;

        pourAnimator.SetFloat("pourSpeed", pourSpeed);
    }
}
