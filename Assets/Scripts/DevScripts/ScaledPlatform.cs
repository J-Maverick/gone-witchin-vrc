
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ScaledPlatform : UdonSharpBehaviour
{
    public PlayerStatBooster playerStatBooster;
    public PlatformPiece[] platforms;
    float currentPlayerJumpRatio = 1;
    float yScale = 1f;
    float yTarget = 1f;
    float xzTarget = 1f;
    float currentPlayerSpeedRatio = 1f;
    float xzScale = 1f;
    float scaleRate = 1f;
    float currentGravityRatio = 1f;
    float prevGravityRatio = 1f;

    public float gravityMultiplier = 1f;
    
    float jumpOffset = 1f;
    float gravityOffset = 1f;
    float speedOffset = 1f;

    private void Start()
    {
        platforms = new PlatformPiece[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            platforms[i] = transform.GetChild(i).GetComponent<PlatformPiece>();
        }

    }

    private void Update()
    {
        if (Time.frameCount % 50 == 0 && Networking.LocalPlayer != null)
        {
            currentPlayerSpeedRatio = Networking.LocalPlayer.GetRunSpeed() / playerStatBooster.defaultRunSpeed;
            currentPlayerJumpRatio = (Networking.LocalPlayer.GetJumpImpulse() / playerStatBooster.defaultJumpImpulse);
            currentGravityRatio = Networking.LocalPlayer.GetGravityStrength() / playerStatBooster.defaultGravityStrength;

            if (yTarget != currentPlayerJumpRatio || xzTarget != currentPlayerSpeedRatio || currentGravityRatio != prevGravityRatio)
            {
                prevGravityRatio = currentGravityRatio;
                jumpOffset = currentPlayerJumpRatio >= 1f ? 1f : 0f;
                gravityOffset = currentGravityRatio >= 1f ? 1f : 0f;
                speedOffset = currentPlayerSpeedRatio >= 1f ? 1f : 0f;
                yTarget = (jumpOffset + 0.9f * (currentPlayerJumpRatio - jumpOffset)) / (gravityOffset + (gravityMultiplier * (Mathf.Sqrt(currentGravityRatio) - gravityOffset)));
                xzTarget = (jumpOffset + (0.75f * (currentPlayerJumpRatio - jumpOffset))) * (speedOffset + (0.9f * (currentPlayerSpeedRatio - speedOffset))) / (gravityOffset + (gravityMultiplier * (currentGravityRatio - gravityOffset)));
                if (yTarget == yScale) scaleRate = 2f;
                else scaleRate = (xzTarget - xzScale) / (yTarget - yScale);
            } 

            Debug.LogFormat("{0}: currentPlayerJumpRatio: {1}, yScale: {2}, yTarget: {3}, xzScale: {4}, xzTarget: {5}, currentGravityRatio: {6}, prevGravityRatio: {7}, gravityMultiplier: {8}", name, currentPlayerJumpRatio, yScale, yTarget, xzScale, xzTarget, currentGravityRatio, prevGravityRatio, gravityMultiplier);
        }

        if (yScale != yTarget || xzScale != xzTarget)
        {
            yScale = Mathf.MoveTowards(yScale, yTarget, Time.deltaTime);
            xzScale = Mathf.MoveTowards(xzScale, xzTarget, Time.deltaTime * scaleRate);
            Vector3 scale = Vector3.one * xzScale;
            scale.y = yScale;
            transform.localScale = scale;

            Vector3 childScale = Vector3.one / Mathf.Sqrt(xzScale);
            childScale.y = 0.5f / yScale;
            foreach (PlatformPiece platform in platforms)
            {
                platform.SetScale(childScale);
            } 
        }

    }
}
