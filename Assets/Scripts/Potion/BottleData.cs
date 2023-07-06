
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// [ExecuteInEditMode]
public class BottleData : UdonSharpBehaviour
{
    public int ID = -1;
    public Mesh mesh = null;
    public float minFill;
    public float maxFill;

    public GameObject thing = null;

    // public void Awake() {
    //     if (mesh != null) {
    //         name = mesh.name;
    //     }

    //     if (thing != null) {
    //         Debug.LogFormat("{0}: got thing!", name);
    //         PotionWobble bottle = thing.GetComponent<PotionWobble>();
    //         minFill = bottle.minFill;
    //         maxFill = bottle.maxFill;
    //     }
    // }
}
