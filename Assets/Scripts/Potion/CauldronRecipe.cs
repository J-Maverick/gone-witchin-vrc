
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CauldronRecipe : Recipe
{
    [UdonSynced] public int nReagentsSync = 0;

    [UdonSynced] public float fillReagent0 = 0f;
    [UdonSynced] public float fillReagent1 = 0f;
    [UdonSynced] public float fillReagent2 = 0f;
    [UdonSynced] public float fillReagent3 = 0f;
    [UdonSynced] public float fillReagent4 = 0f;

    [UdonSynced] public int syncedReagent0ID = -1;
    [UdonSynced] public int syncedReagent1ID = -1;
    [UdonSynced] public int syncedReagent2ID = -1;
    [UdonSynced] public int syncedReagent3ID = -1;
    [UdonSynced] public int syncedReagent4ID = -1;

    public int reagent0ID = -1;
    public int reagent1ID = -1;
    public int reagent2ID = -1;
    public int reagent3ID = -1;
    public int reagent4ID = -1;

    public bool isDS = false;

    public override void OnDeserialization()
    {
        if (reagent0ID != syncedReagent0ID)
        {
            reagent0 = reagentList.GetReagentByID(syncedReagent0ID);
            reagent0ID = syncedReagent0ID;
        }
        if (reagent1ID != syncedReagent1ID)
        {
            reagent1 = reagentList.GetReagentByID(syncedReagent1ID);
            reagent1ID = syncedReagent1ID;
        }
        if (reagent2ID != syncedReagent2ID)
        {
            reagent2 = reagentList.GetReagentByID(syncedReagent2ID);
            reagent2ID = syncedReagent2ID;
        }
        if (reagent3ID != syncedReagent3ID)
        {
            reagent3 = reagentList.GetReagentByID(syncedReagent3ID);
            reagent3ID = syncedReagent3ID;
        }
        if (reagent4ID != syncedReagent4ID)
        {
            reagent4 = reagentList.GetReagentByID(syncedReagent4ID);
            reagent4ID = syncedReagent4ID;
        }
        nReagents = nReagentsSync;
        if (!Networking.IsMaster) isDS = true;
    }

    public override void OnPreSerialization()
    {
        syncedReagent0ID = reagent0ID;
        syncedReagent1ID = reagent1ID;
        syncedReagent2ID = reagent2ID;
        syncedReagent3ID = reagent3ID;
        syncedReagent4ID = reagent4ID;
        nReagentsSync = nReagents;
    }

    public int AddNewReagent(Reagent reagent)
    {
        if (reagent0 == null)
        {
            reagent0 = reagent;
            reagent0ID = reagent0.ID;
            nReagents++;
        }
        else if (reagent1 == null)
        {
            reagent1 = reagent;
            reagent1ID = reagent1.ID;
            nReagents++;
        }
        else if (reagent2 == null)
        {
            reagent2 = reagent;
            reagent2ID = reagent2.ID;
            nReagents++;
        }
        else if (reagent3 == null)
        {
            reagent3 = reagent;
            reagent3ID = reagent3.ID;
            nReagents++;
        }
        else if (reagent4 == null)
        {
            reagent4 = reagent;
            reagent4ID = reagent4.ID;
            nReagents++;
        }
        else return nReagents = -1;

        return nReagents;
    }
    public void AddReagent(Reagent reagent, float fillAmount)
    {
        if (!CheckReagent(reagent) && nReagents != -1) AddNewReagent(reagent);
        if (reagent == reagent0) fillReagent0 += fillAmount;
        else if (reagent == reagent1) fillReagent1 += fillAmount;
        else if (reagent == reagent2) fillReagent2 += fillAmount;
        else if (reagent == reagent3) fillReagent3 += fillAmount;
        else if (reagent == reagent4) fillReagent4 += fillAmount;

        NormalizeReagents();
        RequestSerialization();
    }

    public void NormalizeReagents()
    {
        float[] reagentFills = new float[] { fillReagent0, fillReagent1, fillReagent2, fillReagent3, fillReagent4 };
        float minReagent = MinReagentFill();
        if (minReagent > 0f)
        {
            partsReagent0 = Mathf.RoundToInt(fillReagent0 / minReagent);
            partsReagent1 = Mathf.RoundToInt(fillReagent1 / minReagent);
            partsReagent2 = Mathf.RoundToInt(fillReagent2 / minReagent);
            partsReagent3 = Mathf.RoundToInt(fillReagent3 / minReagent);
            partsReagent4 = Mathf.RoundToInt(fillReagent4 / minReagent);
        }
    }

    public float MinReagentFill()
    {
        float minFill = 99999f;
        if (fillReagent0 > 0f) minFill = fillReagent0;
        if (fillReagent1 > 0f && fillReagent1 < minFill) minFill = fillReagent1;
        if (fillReagent2 > 0f && fillReagent2 < minFill) minFill = fillReagent2;
        if (fillReagent3 > 0f && fillReagent3 < minFill) minFill = fillReagent3;
        if (fillReagent4 > 0f && fillReagent4 < minFill) minFill = fillReagent4;

        if (minFill == 99999f) minFill = -1f;
        return minFill;
    }

    public void LogReagents()
    {
        if (reagent0 == null) Debug.LogFormat("{0}: Current fill: None", name);
        else if (reagent1 == null) Debug.LogFormat("{0}: Current fill: {1} part(s) ({2}) {3}", name, partsReagent0, fillReagent0, reagent0.name);
        else if (reagent2 == null) Debug.LogFormat("{0}: Current fill: {1} part(s) ({2}) {3} + {4} part(s) ({5}) {6}", name, partsReagent0, fillReagent0, reagent0.name, partsReagent1, fillReagent1, reagent1.name);
        else if (reagent3 == null) Debug.LogFormat("{0}: Current fill: {1} part(s) ({2}) {3} + {4} part(s) ({5}) {6} + {7} part(s) ({8}) {9}",
            name, partsReagent0, fillReagent0, reagent0.name, partsReagent1, fillReagent1, reagent1.name, partsReagent2, fillReagent2, reagent2.name);
        else if (reagent4 == null) Debug.LogFormat("{0}: Current fill: {1} part(s) ({2}) {3} + {4} part(s) ({5}) {6} + {7} part(s) ({8}) {9} + {10} part(s) ({11}) {12}",
            name, partsReagent0, fillReagent0, reagent0.name, partsReagent1, fillReagent1, reagent1.name, partsReagent2, fillReagent2, reagent2.name, partsReagent3, fillReagent3, reagent3.name);
        else if (nReagents > 0) Debug.LogFormat("{0}: Current fill: {1} part(s) ({2}) {3} + {4} part(s) ({5}) {6} + {7} part(s) ({8}) {9} + {10} part(s) ({11}) {12} + {13} part(s) ({14}) {15}",
            name, partsReagent0, fillReagent0, reagent0.name, partsReagent1, fillReagent1, reagent1.name, partsReagent2, fillReagent2, reagent2.name, partsReagent3, fillReagent3, reagent3.name, partsReagent4, fillReagent4, reagent4.name);
        else Debug.LogFormat("{0}: Current fill: more than 5 ingredients, too many to brew!", name);
    }
}
