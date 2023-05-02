
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum BoostStackingMode
{
    None = 0,
    Linear = 1,
    Exponential = 2,
    Diminishing = 3,
}

public class PlayerStatBooster : UdonSharpBehaviour
{
    public VRCPlayerApi localPlayer;

    public float defaultJumpImpulse = 3;
    public float defaultWalkSpeed = 2;
    public float defaultRunSpeed = 4;
    public float defaultStrafeSpeed = 2;
    public float defaultGravityStrength = 1;

    public float moveSpeedBoostRatio = 1f;
    public float jumpImpulseBoostRatio = 1f;
    public float gravityStrengthBoostRatio = 1f;

    public bool moveSpeedBoosted = false;
    public bool jumpImpulseBoosted = false;
    public bool gravityStrengthBoosted = false;

    public float moveSpeedReductionRatio = 1f;
    public float jumpImpulseReductionRatio = 1f;
    public float gravityStrengthReductionRatio = 1f;

    public bool moveSpeedReduced = false;
    public bool jumpImpulseReduced = false;
    public bool gravityStrengthReduced = false;

    public float jumpBoostTimer = 0f;
    public float moveSpeedBoostTimer = 0f;
    public float gravityStrengthBoostTimer = 0f;

    public float jumpReductionTimer = 0f;
    public float moveSpeedReductionTimer = 0f;
    public float gravityStrengthReductionTimer = 0f;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        localPlayer.SetRunSpeed(defaultRunSpeed);
        localPlayer.SetWalkSpeed(defaultWalkSpeed);
        localPlayer.SetStrafeSpeed(defaultStrafeSpeed);
        localPlayer.SetJumpImpulse(defaultJumpImpulse);
        localPlayer.SetGravityStrength(defaultGravityStrength);
    }

    public void BoostMoveSpeed(float boostAmount, float boostTime, BoostStackingMode boostStacking)
    {
        moveSpeedBoostTimer += boostTime;

        moveSpeedBoostRatio = GetBoostRatio(moveSpeedBoostRatio, boostAmount, boostStacking);

        SetMoveSpeed();

        moveSpeedBoosted = true;
    }

    public void BoostJumpImpulse(float boostAmount, float boostTime, BoostStackingMode boostStacking)
    {
        jumpBoostTimer += boostTime;

        jumpImpulseBoostRatio = GetBoostRatio(jumpImpulseBoostRatio, boostAmount, boostStacking);

        SetJumpImpulse();

        jumpImpulseBoosted = true;
    }

    public void BoostGravityStrength(float boostAmount, float boostTime, BoostStackingMode boostStacking)
    {
        gravityStrengthBoostTimer += boostTime;

        gravityStrengthBoostRatio = GetBoostRatio(gravityStrengthBoostRatio, boostAmount, boostStacking);

        SetGravityStrength();

        gravityStrengthBoosted = true;
    }

    public void ReduceMoveSpeed(float reductionAmount, float reductionTime, BoostStackingMode reductionStacking)
    {
        moveSpeedReductionTimer += reductionTime;

        moveSpeedReductionRatio = GetReductionRatio(moveSpeedReductionRatio, reductionAmount, reductionStacking);

        SetMoveSpeed();

        moveSpeedReduced = true;
    }

    public void ReduceJumpImpulse(float reductionAmount, float reductionTime, BoostStackingMode reductionStacking)
    {
        jumpReductionTimer += reductionTime;

        jumpImpulseReductionRatio = GetReductionRatio(jumpImpulseReductionRatio, reductionAmount, reductionStacking);

        SetJumpImpulse();

        jumpImpulseReduced = true;
    }

    public void ReduceGravityStrength(float reductionAmount, float reductionTime, BoostStackingMode reductionStacking)
    {
        gravityStrengthReductionTimer += reductionTime;

        gravityStrengthReductionRatio = GetReductionRatio(gravityStrengthReductionRatio, reductionAmount, reductionStacking);

        SetGravityStrength();

        gravityStrengthReduced = true;
    }

    // Assumes currentRatio >= 1, boostAmount > 1
    public float GetBoostRatio(float currentRatio, float boostAmount, BoostStackingMode boostStacking)
    {
        float boostRatio = 1f;
        switch (boostStacking)
        {
            case BoostStackingMode.None:
                if (currentRatio < boostAmount) boostRatio = boostAmount;
                else boostRatio = currentRatio;
                break;
            case BoostStackingMode.Linear:
                float scaledBoost = currentRatio + boostAmount - 1;
                boostRatio = scaledBoost;
                break;
            case BoostStackingMode.Exponential:
                boostRatio = currentRatio * boostAmount;
                break;
            case BoostStackingMode.Diminishing:
                float boostFactor = currentRatio - 1;
                if (boostFactor > boostAmount) boostFactor = Mathf.Sqrt(Mathf.Pow(boostFactor, 2f) + Mathf.Pow(boostAmount, 2f)) + 1;
                else boostFactor = boostFactor + boostAmount;
                boostRatio = boostFactor;
                break;
        }
        return boostRatio;
    }

    // Assumes 0 <= currentRatio <= 1, boostAmount > 1
    public float GetReductionRatio(float currentRatio, float reductionAmount, BoostStackingMode boostStacking)
    {
        float reductionRatio = 1f;
        switch (boostStacking)
        {
            case BoostStackingMode.None:
                if (currentRatio < reductionAmount) reductionRatio = reductionAmount;
                else reductionRatio = currentRatio;
                break;
            case BoostStackingMode.Linear:
                reductionRatio = 1 / ((1 / currentRatio) + reductionAmount);
                break;
            case BoostStackingMode.Exponential:
                reductionRatio = 1 / ((1 / currentRatio) * reductionAmount);
                break;
            case BoostStackingMode.Diminishing:
                float reductionFactor = 1 / currentRatio;
                if (reductionFactor > reductionAmount) reductionFactor = 1 / Mathf.Sqrt(Mathf.Pow(reductionFactor, 2f) + Mathf.Pow(reductionAmount, 2f));
                else reductionFactor = 1 / (reductionFactor + reductionAmount);
                reductionRatio = reductionFactor;
                break;
        }
        return reductionRatio;
    }

    public void SetMoveSpeed()
    {
        localPlayer.SetRunSpeed(defaultRunSpeed * moveSpeedBoostRatio * moveSpeedReductionRatio);
        localPlayer.SetWalkSpeed(defaultWalkSpeed * moveSpeedBoostRatio * moveSpeedReductionRatio);
        localPlayer.SetStrafeSpeed(defaultStrafeSpeed * moveSpeedBoostRatio * moveSpeedReductionRatio);
    }

    public void SetJumpImpulse()
    {
        localPlayer.SetJumpImpulse(defaultJumpImpulse * jumpImpulseBoostRatio * jumpImpulseReductionRatio);
    }

    public void SetGravityStrength()
    {
        localPlayer.SetGravityStrength(defaultGravityStrength * gravityStrengthBoostRatio * gravityStrengthReductionRatio);
    }

    public void ResetMoveSpeed()
    {
        moveSpeedBoostRatio = 1f;
        moveSpeedReductionRatio = 1f;
        moveSpeedBoosted = false;
        moveSpeedBoostTimer = 0f;
        moveSpeedReduced = false;
        moveSpeedReductionTimer = 0f;

    }

    public void ResetJumpImpulse()
    {
        jumpImpulseBoostRatio = 1f;
        jumpImpulseReductionRatio = 1f;
        jumpImpulseBoosted = false;
        jumpBoostTimer = 0f;
        jumpImpulseReduced = false;
        jumpReductionTimer = 0f;
    }

    public void ResetGravityStrength()
    {
        gravityStrengthBoostRatio = 1f;
        gravityStrengthReductionRatio = 1f;
        gravityStrengthBoosted = false;
        gravityStrengthBoostTimer = 0f;
        gravityStrengthReduced = false;
        gravityStrengthReductionTimer = 0f;
    }

    public void ResetAllModifiers()
    {
        ResetMoveSpeed();
        ResetJumpImpulse();
        ResetGravityStrength();
        SetMoveSpeed();
        SetJumpImpulse();
        SetGravityStrength();
    }

    private void Update()
    {
        if (moveSpeedBoosted)
        {
            if (moveSpeedBoostTimer <= 0f)
            {
                moveSpeedBoostRatio = 1f;
                moveSpeedBoosted = false;
                moveSpeedBoostTimer = 0f;
                SetMoveSpeed();
            }
            else moveSpeedBoostTimer -= Time.deltaTime;
        }

        if (moveSpeedReduced)
        {
            if (moveSpeedReductionTimer <= 0f)
            {
                moveSpeedReductionRatio = 1f;
                moveSpeedReduced = false;
                moveSpeedReductionTimer = 0f;
                SetMoveSpeed();
            }
            else moveSpeedReductionTimer -= Time.deltaTime;
        }

        if (jumpImpulseBoosted)
        {
            if (jumpBoostTimer <= 0f)
            {
                jumpImpulseBoostRatio = 1f;
                jumpImpulseBoosted = false;
                jumpBoostTimer = 0f;
                SetJumpImpulse();
            }
            else jumpBoostTimer -= Time.deltaTime;
        }

        if (jumpImpulseReduced)
        {
            if (jumpReductionTimer <= 0f)
            {
                jumpImpulseReductionRatio = 1f;
                jumpImpulseReduced = false;
                jumpBoostTimer = 0f;
                SetJumpImpulse();
            }
            else jumpReductionTimer -= Time.deltaTime;
        }

        if (gravityStrengthBoosted)
        {
            if (gravityStrengthBoostTimer <= 0f)
            {
                gravityStrengthBoostRatio = 1f;
                gravityStrengthBoosted = false;
                gravityStrengthBoostTimer = 0f;
                SetGravityStrength();
            }
            else gravityStrengthBoostTimer -= Time.deltaTime;
        }

        if (gravityStrengthReduced)
        {
            if (gravityStrengthReductionTimer <= 0f)
            {
                gravityStrengthReductionRatio = 1f;
                gravityStrengthReduced = false;
                gravityStrengthReductionTimer = 0f;
                SetGravityStrength();
            }
            else gravityStrengthReductionTimer -= Time.deltaTime;
        }
    }
}
