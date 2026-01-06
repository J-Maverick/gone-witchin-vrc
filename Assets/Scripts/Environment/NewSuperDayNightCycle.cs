
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class NewSuperDayNightCycle : UdonSharpBehaviour
{
    Quaternion startRotation;
    Vector3 rotation;
    public float rotationSpeed;
    public Light sun;
    public float sunIntensity = 3f;
    public float moonIntensity = 1f;
    public float dayNightValue = 0f;
    public Color sunColor;
    public Color moonColor;

    public float bufferAngle = 45f;

    [UdonSynced] public float angle = 45f;
    [UdonSynced] public bool frozen = false;
    public Color daySkyColor;
    public Color dayFogColor;
    public Color tempDayFogColor;
    public bool useTempDayFogColor;
    public Color nightFogColor;
    public float dayFogDensity;
    public float nightFogDensity;

    public AudioSource dayAudio;
    public AudioSource nightAudio;
    
    public FishingZone sunZone;
    public FishingZone moonZone;
    public Collider[] zoneColliders;

    void Start()
    {
        startRotation = transform.localRotation;
        rotation = transform.localRotation.eulerAngles;
        RenderSettings.skybox.SetColor("_SkyTint", daySkyColor);
        SetSun();
        OnDeserialization();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }

    public void DisableSun() {
        sun.enabled = false;
    }

    public void EnableSun() {
        sun.enabled = true;
    }

    public override void OnDeserialization() {
        Debug.LogFormat("{0}: OnDeserialization", name);
        float modAngle = angle % 360f;
        if (modAngle > bufferAngle && modAngle < 180f - bufferAngle ) {
            sun.intensity = sunIntensity;
            dayAudio.volume = 0.5f;
            nightAudio.volume = 0f;
            RenderSettings.fogDensity = dayFogDensity;
            RenderSettings.fogColor = useTempDayFogColor ? tempDayFogColor : dayFogColor;
        }
        else if (modAngle < 360f - bufferAngle && modAngle > 180f - bufferAngle) {
            sun.intensity = moonIntensity;
            dayAudio.volume = 0f;
            nightAudio.volume = 0.5f;
            RenderSettings.fogDensity = nightFogDensity;
            RenderSettings.fogColor = nightFogColor;
        }
    }

    public void SetTempColor(Color fogColor, Color skyColor) {
        tempDayFogColor = fogColor;
        RenderSettings.skybox.SetColor("_SkyTint", skyColor);
        useTempDayFogColor = true;
    }

    public void ResetTempColor() {
        useTempDayFogColor = false;
        RenderSettings.skybox.SetColor("_SkyTint", daySkyColor);
    }

    public void SetAngle(float newAngle) {
        angle = newAngle;
        UpdateRotation();
        CheckRiseSet();
    }

    public void FinalizeAngle(float newAngle) {
        angle = newAngle;
        UpdateRotation();
        CheckRiseSet();
        RequestSerialization();
        SendCustomEventDelayedSeconds(nameof(DelayedRequestSerialization), 0.5f);
    }

    public void DelayedRequestSerialization() {
        RequestSerialization();
    }

    void UpdateRotation() {
        rotation.x = angle;
        transform.localRotation = Quaternion.Euler(rotation);
    }

    void CheckRiseSet() {
        float modAngle = angle % 360f;
        if (modAngle < 180f) {
            sun.transform.localRotation = Quaternion.Euler(0, 0, 0);
            float sunValue = Mathf.Sqrt(8100f - Mathf.Pow(modAngle - 90f, 2f)) / 90f;
            dayNightValue = -sunValue;
            sun.intensity = sunIntensity * sunValue;
            sun.color = sunColor;
            dayAudio.volume = 0.5f * sunValue;
            nightAudio.volume = 0f;
            SetSun();
        }
        else {
            sun.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            float sunValue = Mathf.Sqrt(8100f - Mathf.Pow(modAngle - 270f, 2f)) / 90f;
            dayNightValue = sunValue;
            sun.intensity = moonIntensity * sunValue;
            sun.color = moonColor;
            dayAudio.volume = 0f;
            nightAudio.volume = 0.5f * sunValue;
            SetMoon();
        }
        if (modAngle < bufferAngle) {
            float lerpValue = (bufferAngle + modAngle) / (2f * bufferAngle);
            RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, lerpValue);
            RenderSettings.fogColor = Color.Lerp(nightFogColor, useTempDayFogColor ? tempDayFogColor : dayFogColor, lerpValue);
        }
        else if (modAngle > 360f - bufferAngle) {
            float lerpValue = (modAngle - (360f - bufferAngle)) / (2f * bufferAngle);
            RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, lerpValue);
            RenderSettings.fogColor = Color.Lerp(nightFogColor, useTempDayFogColor ? tempDayFogColor : dayFogColor, lerpValue);
        }
        else if (modAngle > 180f - bufferAngle && modAngle < 180f + bufferAngle) {
            float lerpValue = (modAngle - (180f - bufferAngle)) / (2f * bufferAngle);
            RenderSettings.fogDensity = Mathf.Lerp(dayFogDensity, nightFogDensity, lerpValue);
            RenderSettings.fogColor = Color.Lerp(useTempDayFogColor ? tempDayFogColor : dayFogColor, nightFogColor, lerpValue);
        }
        else if (modAngle > 180f) {
            RenderSettings.fogDensity = nightFogDensity;
            RenderSettings.fogColor = nightFogColor;
        }
        else {
            RenderSettings.fogDensity = dayFogDensity;
            RenderSettings.fogColor = useTempDayFogColor ? tempDayFogColor : dayFogColor;
        }
    }

    void Update() {
        if (frozen) {
            return;
        }
        angle += rotationSpeed * Time.deltaTime;
        UpdateRotation();
        CheckRiseSet();
    }

    public void Freeze()
    {
        frozen = true;
        RequestSerialization();
    }

    public void Unfreeze()
    {
        frozen = false;
        RequestSerialization();
    }

    public void SetSun() {
        if (!sunZone.zoneActive && Networking.GetOwner(sunZone.gameObject).isLocal) {
            int randColliderIndex = Random.Range(0, zoneColliders.Length);
            Vector3 position = RandomPointInBounds(zoneColliders[randColliderIndex].bounds);
            position.y = sunZone.zonePosition.y;
            sunZone.Activate(position);
            moonZone.DeActivate();
        }
    }

    public void SetMoon() {
        if (!moonZone.zoneActive && Networking.GetOwner(moonZone.gameObject).isLocal) {
            int randColliderIndex = Random.Range(0, zoneColliders.Length);
            Vector3 position = RandomPointInBounds(zoneColliders[randColliderIndex].bounds);
            position.y = moonZone.zonePosition.y;
            moonZone.Activate(position);
            sunZone.DeActivate();
        }
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
