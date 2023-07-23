
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PotionOcean : UdonSharpBehaviour
{
    public PotionPool[] potionPools;

    public GameObject TryToSpawnByID(int ID) {
        foreach (PotionPool potionPool in potionPools) {
            if (potionPool.IsMatch(ID)) {
                Networking.SetOwner(Networking.LocalPlayer, potionPool.gameObject);
                return potionPool.pool.TryToSpawn();
            }
        }
        return null;
    }
}
