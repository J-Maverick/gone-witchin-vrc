
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

enum IndicatorState
{
    invalid = 0,
    valid = 1,
    neutral = 2
};

public class GemIndicator : UdonSharpBehaviour
{
    public Renderer rend;
    public int emissionMultiplier = 4;
    public AudioSource invalidAudio = null;
    private Material material;
    private Color neutralColor = Color.white;
    private Color invalidColor = Color.red;
    private Color validColor = Color.green;
    private IndicatorState state = IndicatorState.neutral;

    private void Start()
    {
        material = rend.materials[0];
        // material.color = neutralColor;
        material.SetColor("_EmissionColor", neutralColor * 0f);
    }

    public void SetValid()
    {
        if (state != IndicatorState.valid)
        {
            // material.color = validColor;
            material.SetColor("_EmissionColor", validColor * emissionMultiplier);
            state = IndicatorState.valid;
        }
    }

    public void SetInvalid(bool playSound=true)
    {
        if (state != IndicatorState.invalid)
        {
            // material.color = invalidColor;
            material.SetColor("_EmissionColor", invalidColor * emissionMultiplier);
            state = IndicatorState.invalid;
        }
        if (playSound && invalidAudio != null) {
            invalidAudio.Play();
        }
    }

    public void SetNeutral()
    {
        if (state != IndicatorState.neutral)
        {
            // material.color = neutralColor;
            material.SetColor("_EmissionColor", neutralColor * 0f);
            state = IndicatorState.neutral;
        }
    }

    public bool IsValid()
    {
        return state == IndicatorState.valid;
    }

    public bool IsNeutral()
    {
        return state == IndicatorState.neutral;
    }
}
