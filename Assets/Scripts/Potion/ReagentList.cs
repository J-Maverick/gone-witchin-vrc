
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReagentList : UdonSharpBehaviour
{
    public Reagent[] reagents;

    public Reagent GetReagentByID(int ID)
    {
        foreach (Reagent reagent in reagents)
        {
            if (reagent.ID == ID) return reagent;
        }
        return null;
    }
}
