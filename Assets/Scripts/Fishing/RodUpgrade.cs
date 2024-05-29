
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RodUpgrade : UdonSharpBehaviour
{   
    public float castTension = 1f;
    public float castWeight = 1f;
    public float castDrag = 5f;

    public float catchTension = 750f;
    public float catchWeight = 200f;
    public float catchDrag = 10f;

    public float fishOnSpringRatio = 0.05f;
    public float castSpringRatio = 0.002f;
    public float castDistanceRatio = 0.006f;
    public float fishOnDistanceRatio = 0.00025f;
    public float maxSpring = 2500f;
    public float reelingInactiveTime = 10f;
    public float fishExhaustionMultiplier = 1f;

    public float waitTimeModifier = 1f;

    public int level = 0;

    public Mesh mesh = null;
    public Material material = null;

}
