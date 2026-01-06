
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
    public AudioSource resetAudioSource = null;
    public Collider selfCollider = null;
    public UdonSharpBehaviour activatableComponent = null;
    public UdonSharpBehaviour[] activatableComponents = null;

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

    public void Reset()
    {
        Destroyed = false;
        if (meshObject != null) {
            meshObject.SetActive(true);
        }
        if (selfCollider != null) {
            selfCollider.enabled = true;
        }
        if (resetAudioSource != null) {
            resetAudioSource.enabled = true;
            if (Vector3.Distance(Networking.LocalPlayer.GetPosition(), transform.position) < resetAudioSource.maxDistance)
            {
                resetAudioSource.Play();
            }
            SendCustomEventDelayedSeconds(nameof(TryDisableResetAudioSource), resetAudioSource.clip.length + 0.1f);
        }
        RequestSerialization();
    }

    void MyMainGoalIsToBlowUp() {
        if (meshObject != null) {
            meshObject.SetActive(false);
        }
        if (audioSource != null) {
            audioSource.enabled = true;
            if (Vector3.Distance(Networking.LocalPlayer.GetPosition(), transform.position) < audioSource.maxDistance)
            {
                audioSource.Play();
            }
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
        if (activatableComponents != null) {
            for (int i = 0; i < activatableComponents.Length; i++) {
                activatableComponents[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Activate");
            }
        }
        RequestSerialization();

    }

    public void TryDisableAudioSource()
    {
        if (audioSource.isPlaying) return;
        audioSource.enabled = false;
    }

    public void TryDisableResetAudioSource()
    {
        if (resetAudioSource.isPlaying) return;
        resetAudioSource.enabled = false;
    }
}
