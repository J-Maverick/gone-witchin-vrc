
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PotionWobble : UdonSharpBehaviour
{ 
    public Renderer rend;
    public float maxFill = 1f;
    public float minFill = 0f;
    
    public float fillLevel = 1f;

    private Material material;
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
    float time = 0.5f;

    private void Start()
    {
        if (isPotion)
        material = rend.materials[1];
        else
        material = rend.materials[0];
    }

    public void SetColor(Color bottleColor)
    {
        if (material == null) material = rend.materials[1];
        material.color = Color.Lerp(material.GetColor("_Tint"), bottleColor, 0.9f);
    }

    public void SetStaticColor(Color bottleColor)
    {
        if (material == null) material = rend.materials[1];
        material.color = bottleColor;
    }

    public void UpdateFillColor(Color bottleColor, float fillAmount)
    {
        if (material == null) material = rend.materials[1];
        material.color = Color.Lerp(material.color, bottleColor, fillAmount / fillLevel);
    }

    public void UpdateFillLevel()
    {
        float fill;
        if (fillLevel > 0) fill = minFill + (maxFill - minFill) * fillLevel;
        else fill = 1f;
        material.SetFloat("_FillAmount", fill);
    }

    public Color GetColor()
    {
        return material.color;
    }

    private void Update()
    {
        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        // send it to the shader
        material.SetFloat("_WobbleX", wobbleAmountX);
        material.SetFloat("_WobbleZ", wobbleAmountZ);


        if (isPotion)
        {

            // velocity
            velocity = (lastPos - transform.position) / Time.deltaTime;
            angularVelocity = transform.rotation.eulerAngles - lastRot;

            // add clamped velocity to wobble
            wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
            wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

            // keep last position
            lastPos = transform.position;
            lastRot = transform.rotation.eulerAngles;
        }
        UpdateFillLevel();
    }
}