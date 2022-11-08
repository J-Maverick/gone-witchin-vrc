
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RigidBodyCorrector : UdonSharpBehaviour
{
    Vector3 correctedPosition;
    void Start()
    {
        correctedPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        transform.localPosition = correctedPosition;
    }
}
