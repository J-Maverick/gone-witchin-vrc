
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BottleDataList : UdonSharpBehaviour
{
    public BottleData[] bottles;

    public BottleData GetBottleByID(int ID) {

        foreach (BottleData bottle in bottles) {
            if (ID == bottle.ID) {
                return bottle;
            }
        }

        return null;
    }
}
