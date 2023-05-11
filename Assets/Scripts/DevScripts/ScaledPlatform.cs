
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ScaledPlatform : UdonSharpBehaviour
{
    public PlayerStatBooster playerStatBooster;
    public PlatformPiece[] platforms;
    float currentPlayerJumpRatio = 1;
    float yScale = 1;
    float currentPlayerSpeedRatio = 1;
    float xzScale = 1;

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
        currentPlayerSpeedRatio = Networking.LocalPlayer.GetRunSpeed() / playerStatBooster.defaultRunSpeed;
        currentPlayerJumpRatio = Networking.LocalPlayer.GetJumpImpulse() / playerStatBooster.defaultJumpImpulse;

        if (Time.frameCount % 50 == 0)
        {
            if (yScale != currentPlayerJumpRatio || xzScale != currentPlayerSpeedRatio)
            {
                yScale = 1 + (0.9f * (currentPlayerJumpRatio - 1));
                xzScale = (1 + (0.75f * (currentPlayerJumpRatio - 1))) * (1 + (0.9f * (currentPlayerSpeedRatio - 1)));
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
}
