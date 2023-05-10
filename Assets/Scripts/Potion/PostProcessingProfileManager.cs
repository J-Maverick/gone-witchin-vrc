
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PostProcessingProfileManager : UdonSharpBehaviour
{
    public GameObject defaultVolume;
    public GameObject bloomVolume;
    public GameObject catseyeVolume;
    public GameObject dogeyeVolume;
    public GameObject gloomVolume;
    public GameObject kaleidoVolume;
    public GameObject loveVolume;
    public GameObject nostalgiaVolume;
    public GameObject rosyVolume;

    public void ClearPostProcessing()
    {    
        defaultVolume.SetActive(false);
        bloomVolume.SetActive(false);
        catseyeVolume.SetActive(false);
        dogeyeVolume.SetActive(false);
        gloomVolume.SetActive(false);
        kaleidoVolume.SetActive(false);
        loveVolume.SetActive(false);
        nostalgiaVolume.SetActive(false);
        rosyVolume.SetActive(false);
    }

    public void Default()
    {
        ClearPostProcessing();
        defaultVolume.SetActive(true);
    }

    public void Bloom()
    {
        ClearPostProcessing();
        bloomVolume.SetActive(true);
    }

    public void Catseye()
    {
        ClearPostProcessing();
        catseyeVolume.SetActive(true);
    }

    public void Dogeye()
    {
        ClearPostProcessing();
        dogeyeVolume.SetActive(true);
    }

    public void Gloom()
    {
        ClearPostProcessing();
        gloomVolume.SetActive(true);
    }

    public void Kaleido()
    {
        float animSpeed = 1f;
        Animator animator = kaleidoVolume.GetComponent<Animator>();
        if (kaleidoVolume.activeSelf)
        {
            animSpeed = animator.GetFloat("AnimationSpeed") * 2f;
        }
        ClearPostProcessing();
        kaleidoVolume.SetActive(true);
        animator.SetFloat("AnimationSpeed", animSpeed);
    }

    public void Love()
    {
        ClearPostProcessing();
        loveVolume.SetActive(true);
    }

    public void Nostalgia()
    {
        ClearPostProcessing();
        nostalgiaVolume.SetActive(true);
    }

    public void Rosy()
    {
        ClearPostProcessing();
        rosyVolume.SetActive(true);
    }

}
