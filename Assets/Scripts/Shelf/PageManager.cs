
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PageManager : UdonSharpBehaviour
{
    public GameObject[] pages;
    

    public Color activeColor;
    public Color inactiveColor;

    public void SetPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }
    }

    public void NextPage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].activeSelf)
            {
                int nextIndex = (i + 1) % pages.Length;
                SetPage(nextIndex);
                break;
            }
        }
    }
    
    public void PreviousPage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].activeSelf)
            {
                int prevIndex = (i - 1 + pages.Length) % pages.Length;
                SetPage(prevIndex);
                break;
            }
        }
    }
}
