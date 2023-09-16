
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlatformPiece : UdonSharpBehaviour
{
    public Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
        Vector3 pos = startPosition;
        pos.y *= 1 / (2f * scale.y);
        transform.localPosition = pos;
    }

}
