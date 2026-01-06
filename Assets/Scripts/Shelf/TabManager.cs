
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class TabManager : UdonSharpBehaviour
{
    public GameObject[] tabs;
    public Image[] tabButtons;

    public Color activeColor;
    public Color inactiveColor;

    public void SetTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(i == index);
            tabButtons[i].color = i == index ? activeColor: inactiveColor;
        }
    }

    public void SetTab0() { SetTab(0); }
    public void SetTab1() { SetTab(1); }
    public void SetTab2() { SetTab(2); }
    public void SetTab3() { SetTab(3); }
    public void SetTab4() { SetTab(4); }
    public void SetTab5() { SetTab(5); }
    public void SetTab6() { SetTab(6); }
    public void SetTab7() { SetTab(7); }
    public void SetTab8() { SetTab(8); }
    public void SetTab9() { SetTab(9); }
    public void SetTab10() { SetTab(10); }
}
