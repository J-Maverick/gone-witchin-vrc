
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AmbienceZone : UdonSharpBehaviour
{
    public AudioSource[] sources;
    public float switchDistance = 150f;

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player.isLocal) {
            float distance = (player.GetPosition() - transform.position).magnitude;
            float lerp = (distance - switchDistance) / ((transform.localScale.x / 2f) - switchDistance);
            foreach (AudioSource source in sources) {
                if (lerp <= 0f) {
                    // source.spatialize = false;
                    source.spatialBlend = 0.0f;
                }
                else {
                    // source.spatialize = true;
                    source.spatialBlend = lerp;
                }
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        foreach (AudioSource source in sources) {
            // source.spatialize = true;
            source.spatialBlend = 1.0f;
        }
    }
}
