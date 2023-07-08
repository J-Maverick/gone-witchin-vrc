
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum BottleType
{
    None = 0,
    Reagent = 1,
    Potion = 2
}

public class Bottle : UdonSharpBehaviour
{
    public Color potionColor;
    public PotionWobble shaderControl = null;
    public float fillLevel = 0f;
    public BottleType type = BottleType.None;
    public LiquidMaterial liquid = null;

    public BottleSpawner spawner = null;

    public int bottleID = 2;

    protected virtual void Start()
    {
        shaderControl = GetComponent<PotionWobble>();
        if (liquid != null) shaderControl.SetStaticColor(potionColor);
    }

    public void Despawn() {
        if (spawner != null) {
            spawner.Despawn(gameObject);
        }
        // else {
        //     gameObject.SetActive(false);
        // }
    }

}
