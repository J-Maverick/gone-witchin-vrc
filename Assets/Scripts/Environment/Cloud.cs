
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Cloud : UdonSharpBehaviour
{
    public Rigidbody rigidBody;

    public float minX = -2500f;
    public float maxX = 2500f;
    public float velocity = 5f;
    public float velocityVariation = 2f;
    public float delayTime = 10f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = new Vector3(Random.Range(velocity - velocityVariation, velocity + velocityVariation), 0, 0);
        SendCustomEventDelayedSeconds(nameof(CheckDistance), Random.Range(0f, delayTime));
    }


    public void CheckDistance()
    {
        if (transform.position.x > maxX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }
        SendCustomEventDelayedSeconds(nameof(CheckDistance), delayTime);
    }

}
