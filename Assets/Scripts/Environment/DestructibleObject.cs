
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DestructibleObject : UdonSharpBehaviour
{
    public GameObject meshObject = null;
    public ParticleSystem destructionParticles = null;
    public AudioSource audioSource = null;
    public Collider selfCollider = null;

    [UdonSynced, FieldChangeCallback(nameof(Destroyed))] 
    private bool _destroyed = false;

    public bool Destroyed {
        set {
            if (value != _destroyed) {
                    _destroyed = value;
                    if (_destroyed) {                  
                        MyMainGoalIsToBlowUp();
                    }
                }
            }

        get => _destroyed;
    }

    public void Destruct() {
        Debug.LogFormat("{0}: Destruction triggered", name);
        Destroyed = true;
    }

    void MyMainGoalIsToBlowUp() {
        if (meshObject != null) {
            meshObject.SetActive(false);
        }
        if (audioSource != null) {
            audioSource.Play();
        }
        if (destructionParticles != null) {
            destructionParticles.Play();
        }
        if (selfCollider != null) {
            selfCollider.enabled = false;
        }
        RequestSerialization();
    }
}
