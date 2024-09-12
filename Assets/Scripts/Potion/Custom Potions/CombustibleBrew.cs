
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CombustibleBrew : ShatterEffect
{
    public float radius = 5.0F;
    public float power = 500.0F;
    public BottleCollision bottleCollision;

    public override void OnShatter() {
        Explode();
        bottleCollision.PlaySoundEffect();
    }

    void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            if (hit != null) {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(power, explosionPos, radius, 3.0F);

                DestructibleObject destructibleObject = hit.GetComponent<DestructibleObject>();
                if (destructibleObject != null) {
                    Debug.LogFormat("{0}: Found destructible object, blowin it up!", name);
                    destructibleObject.Destruct();
                }
                IceBlock iceBlock = hit.GetComponent<IceBlock>();
                if (iceBlock != null) {
                    Debug.LogFormat("{0}: Found ice block, blowin it up!", name);
                    iceBlock.Despawn();
                }
            }
        }
    }
}
