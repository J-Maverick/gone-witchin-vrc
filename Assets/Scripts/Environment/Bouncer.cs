
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Bouncer : UdonSharpBehaviour
{
    public float bounceMultiplier = 1f;
    public float maxBounce = 100f;
    public float minBounceImpulse = 1f;
    public float minBounceSpeed = 0f;
    public Animator animator = null;

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        Debug.LogFormat("{0}: Bouncing player: {1}[{2}]", name, player.displayName, player.playerId);
        if (player.isLocal)
        {
            Bounce(player);
        }
    }

    public void Bounce(VRCPlayerApi player)
    {
        Vector3 velocity = Networking.LocalPlayer.GetVelocity();
        if (minBounceSpeed > velocity.y)
        {
            animator.SetBool("Bounce", true);
            velocity.y = Mathf.Abs(velocity.y * bounceMultiplier);
            if (velocity.y > maxBounce)
            {
                velocity.y = maxBounce;
            }
            else if (velocity.y < minBounceImpulse)
            {
                velocity.y = minBounceImpulse;
            }
            Networking.LocalPlayer.SetVelocity(velocity);
            SendCustomEventDelayedSeconds("ResetAnimator", 0.1f);
        }
    }

    public void ResetAnimator()
    {
        animator.SetBool("Bounce", false);
    }
}
