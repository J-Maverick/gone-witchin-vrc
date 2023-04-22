
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
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
        }
        explode = false;
    }

    private void Update()
    {
        if (explode) Explode();
    }
}