
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
    public UdonSharpBehaviour activatableComponent = null;

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
            audioSource.enabled = true;
            audioSource.Play();
            SendCustomEventDelayedSeconds(nameof(TryDisableAudioSource), audioSource.clip.length + 0.1f);
        }
        if (destructionParticles != null) {
            destructionParticles.Play();
        }
        if (selfCollider != null) {
            selfCollider.enabled = false;
        }
        if (activatableComponent != null) {
            activatableComponent.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Activate");
        }
        RequestSerialization();

    }

    public void TryDisableAudioSource()
    {
        if (audioSource.isPlaying) return;
        audioSource.enabled = false;
    }
}
