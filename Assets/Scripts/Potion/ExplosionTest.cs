
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ExplosionTest : UdonSharpBehaviour
{
    public float radius = 5.0F;
    public float power = 10.0F;
    public bool explode = false;

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
            }
        }
        explode = false;
    }

    private void Update()
    {
        if (explode) Explode();
    }
}