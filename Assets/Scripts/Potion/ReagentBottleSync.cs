
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ReagentBottleSync : BottleSync
{
    public ReagentList reagentList;
    [UdonSynced] public int reagentID = -1;
    public ReagentBottle reagentBottle;

    public override void OnPreSerialization()
    {
        if (reagentBottle != null && reagentBottle.reagent != null)
        reagentID = reagentBottle.reagent.ID;
        base.OnPreSerialization();
    }

    public override void OnDeserialization()
    {
        if (reagentBottle != null && reagentID != -1)
        {
            if (reagentBottle.reagent == null || reagentBottle.reagent.ID != reagentID)
            {
                reagentBottle.reagent = reagentList.GetReagentByID(reagentID);
                reagentBottle.UpdateReagentProperties();
            }
            reagentBottle.fillLevel = fillLevel;
            pourableBottle.UpdateShaderFill();
        }
        base.OnDeserialization();
    }
}
