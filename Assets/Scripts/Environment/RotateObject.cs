
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RotateObject : UdonSharpBehaviour
{
    public float XSpeed = 0f;
    public float YSpeed = 0f;
    public float ZSpeed = 0f;
    void Start()
    {
        
    }

    private void Update()
    {
        transform.Rotate(XSpeed * Time.deltaTime, YSpeed * Time.deltaTime, ZSpeed * Time.deltaTime);
    }
}
