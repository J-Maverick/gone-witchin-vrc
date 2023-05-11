
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
    float currentPlayerSpeedRatio = 1f;
    float xzTarget = 1f;
    float xzScale = 1f;
    float scaleRate = 1f;

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

        if (Time.frameCount % 50 == 0)
        {
            currentPlayerSpeedRatio = Networking.LocalPlayer.GetRunSpeed() / playerStatBooster.defaultRunSpeed;
            currentPlayerJumpRatio = Networking.LocalPlayer.GetJumpImpulse() / playerStatBooster.defaultJumpImpulse;

            if (yTarget != currentPlayerJumpRatio || xzTarget != currentPlayerSpeedRatio)
            {
                yTarget = 1 + (0.9f * (currentPlayerJumpRatio - 1));
                xzTarget = (1 + (0.75f * (currentPlayerJumpRatio - 1))) * (1 + (0.9f * (currentPlayerSpeedRatio - 1)));
                if (yTarget == yScale) scaleRate = 2f;
                else scaleRate = (xzTarget - xzScale) / (yTarget - yScale);
            }
        }

        if (yScale != yTarget || xzScale != xzTarget)
        {
            yScale = Mathf.MoveTowards(yScale, yTarget, Time.deltaTime);
            xzScale = Mathf.MoveTowards(xzScale, xzTarget, Time.deltaTime * scaleRate);
            Vector3 scale = Vector3.one * xzScale;
            scale.y = yScale;
            transform.localScale = scale;

            Vector3 childScale = Vector3.one / Mathf.Sqrt(xzScale);
            childScale.y = 1f / yScale;
            foreach (PlatformPiece platform in platforms)
            {
                platform.SetScale(childScale);
            }
        }
    }
}
