
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CraftedBaitOcean : UdonSharpBehaviour
{
    public CraftedBaitPool[] pools;

    public CraftedBaitPool GetPoolByBait(Bait bait) {
        foreach (CraftedBaitPool pool in pools) {
            if (pool.bait == bait) {
                return pool;
            }
        }
        return null;
    }
}
