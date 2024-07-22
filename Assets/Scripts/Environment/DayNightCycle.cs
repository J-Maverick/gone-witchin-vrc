
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DayNightCycle : UdonSharpBehaviour
{
    Quaternion startRotation;
    Vector3 rotation;
    public float rotationSpeed;
    public Light sun;
    public float sunIntensity = 0f;
    public Light moon;
    public float moonIntensity = 0f;

    public float bufferAngle = 10f;

    [UdonSynced]
    public float angle = 0f;
    public Color dayFogColor;
    public Color nightFogColor;
    public float dayFogDensity;
    public float nightFogDensity;

    public AudioSource dayAudio;
    public AudioSource nightAudio;

    void Start()
    {
        startRotation = transform.localRotation;
        rotation = transform.localRotation.eulerAngles;
        sunIntensity = sun.intensity;
        moonIntensity = moon.intensity;
        OnDeserialization();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }

    public override void OnDeserialization() {
        float modAngle = angle % 360f;
        if (modAngle > bufferAngle && modAngle < 180f - bufferAngle ) {
            sun.intensity = sunIntensity;
            moon.intensity = 0f;
        }
        else if (modAngle < 360f - bufferAngle && modAngle > 180f - bufferAngle) {
            sun.intensity = 0f;
            moon.intensity = moonIntensity;
        }
    }

    void Update() {
        angle += rotationSpeed * Time.deltaTime;
        rotation.x = angle;
        transform.localRotation = Quaternion.Euler(rotation);
        float modAngle = angle % 360f;
        if (modAngle < bufferAngle) {
            float lerpValue = (bufferAngle + modAngle) / (2f * bufferAngle);
            sun.intensity = Mathf.Lerp(0f, sunIntensity, lerpValue );
            moon.intensity = Mathf.Lerp(moonIntensity, 0f, lerpValue);
            dayAudio.volume = Mathf.Lerp(0f, 0.5f, lerpValue);
            nightAudio.volume = Mathf.Lerp(0.5f, 0.0f, lerpValue);
            RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, lerpValue);
            RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, lerpValue);
        }
        else if (modAngle > 360f - bufferAngle) {
            float lerpValue = (modAngle - (360f - bufferAngle)) / (2f * bufferAngle);
            sun.intensity = Mathf.Lerp(0f, sunIntensity, lerpValue);
            moon.intensity = Mathf.Lerp(moonIntensity, 0f, lerpValue);
            dayAudio.volume = Mathf.Lerp(0f, 0.5f, lerpValue);
            nightAudio.volume = Mathf.Lerp(0.5f, 0.0f, lerpValue);
            RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, lerpValue);
            RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, lerpValue);
        }
        else if (modAngle > 180f - bufferAngle && modAngle < 180f + bufferAngle) {
            float lerpValue = (modAngle - (180f - bufferAngle)) / (2f * bufferAngle);
            sun.intensity = Mathf.Lerp(sunIntensity, 0f, lerpValue);
            moon.intensity = Mathf.Lerp(0f, moonIntensity, lerpValue);
            dayAudio.volume = Mathf.Lerp(0.5f, 0f, lerpValue);
            nightAudio.volume = Mathf.Lerp(0f, 0.5f, lerpValue);
            RenderSettings.fogDensity = Mathf.Lerp(dayFogDensity, nightFogDensity, lerpValue);
            RenderSettings.fogColor = Color.Lerp(dayFogColor, nightFogColor, lerpValue);
        }
    }
}
