
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PotionWobble : UdonSharpBehaviour
{ 
    public Renderer rend;
    public float maxFill = 1f;
    public float minFill = 0f;
    
    [FieldChangeCallback(nameof(fillLevel))]
    private float _fillLevel = 1f;

    public float fillLevel 
    {
        set
        {
            _fillLevel = value;
            UpdateFillLevel();
        }
        get => _fillLevel;
    }

    private Material material;
    public Rigidbody rigidBody;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;  
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;

    public bool isPotion = true;

    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;

    bool wobbleActive = true;

    private void Start()
    {
        if (isPotion)
        material = rend.materials[1];
        else
        material = rend.materials[0];
        UpdateFillLevel();
    }

    public void SetColor(Color newColor)
    {
        if (material == null)
        {
            if (isPotion) material = rend.materials[1];
            else material = rend.materials[0];
        }
        material.color = Color.Lerp(material.GetColor("_Tint"), newColor, 0.9f);
    }

    public void SetStaticColor(Color newColor)
    {
        if (material == null) material = rend.materials[1];
        material.color = newColor;
    }

    public void UpdateFillLevel()
    {
        float fill;
        if (_fillLevel > 0) fill = minFill + (maxFill - minFill) * _fillLevel;
        else fill = 1f;
        material.SetFloat("_FillAmount", fill);
    }

    public void FillBump(float scale=1f)
    {
        wobbleAmountToAddX += Random.Range(0, 0.0015f * scale);
        wobbleAmountToAddZ += Random.Range(0, 0.0015f * scale);
        wobbleActive = true;
    }

    public Color GetColor()
    {
        return material.color;
    }

    public void Wobble() {
        if (wobbleActive) // Sleepy boi logic
        {
            // decrease wobble over time
            wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
            wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));
            if (Mathf.Abs(wobbleAmountToAddX) < 0.001) wobbleAmountToAddX = 0f;
            if (Mathf.Abs(wobbleAmountToAddZ) < 0.001) wobbleAmountToAddZ = 0f;

            // make a sine wave of the decreasing wobble
            pulse = 2f * Mathf.PI * WobbleSpeed;
            wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * Time.realtimeSinceStartup);
            wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * Time.realtimeSinceStartup);

            // send it to the shader
            material.SetFloat("_WobbleX", wobbleAmountX);
            material.SetFloat("_WobbleZ", wobbleAmountZ);

            if (wobbleAmountToAddX == 0f && wobbleAmountToAddZ == 0f) {
                wobbleActive = false;
            }
        }
    }

    public void MovementWobble() {
        if (!rigidBody.IsSleeping())
        {
            //// velocity
            velocity = (lastPos - transform.position) / Time.deltaTime;
            angularVelocity = transform.rotation.eulerAngles - lastRot;
            Debug.LogFormat("{0}: Movement Wobble!", name);

            // add clamped velocity to wobble
            wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
            wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

            // keep last position
            lastPos = transform.position;
            lastRot = transform.rotation.eulerAngles;
            wobbleActive = true;
        }
    }

    public virtual void Update()
    {
        Wobble();
        MovementWobble();
    }
}
