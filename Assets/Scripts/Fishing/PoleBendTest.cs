
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PoleBendTest : UdonSharpBehaviour
{
    float x = 0f;
    float z = 0f;
    public Animator poleAnimator;
    public Transform poleCenter;
    public Transform poleTarget;

    private void Update()
    {
        

        poleAnimator.SetFloat("PoleX", x);
        poleAnimator.SetFloat("PoleZ", z);
    }
}
