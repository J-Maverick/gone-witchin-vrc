
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ReagentBottleSync : BottleSync
{
    public LiquidList liquidList;
    [UdonSynced] public int reagentID = -1;
    public ReagentBottle reagentBottle;

    public override void OnPreSerialization()
    {
        if (reagentBottle != null && reagentBottle.liquid != null) reagentID = reagentBottle.liquid.ID;
        else reagentID = -1;

        base.OnPreSerialization();
    }

    public override void OnDeserialization()
    {
        if (reagentBottle != null && reagentID != -1)
        {
            if (reagentBottle.liquid == null || reagentBottle.liquid.ID != reagentID)
            {
                reagentBottle.liquid = liquidList.GetLiquidByID(reagentID);
                reagentBottle.UpdateLiquidProperties();
            }
            reagentBottle.fillLevel = fillLevel;
            pourableBottle.UpdateShaderFill();
        }
        base.OnDeserialization();
    }
}
