
using Cysharp.Threading.Tasks.Triggers;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class StoveSnap : UdonSharpBehaviour
{
    //public Stove stove
    public Animator animator;
    public GemIndicator indicator;
    public float cooldownTime = 1f;
    public bool coolingDown = false;
    public Transform[] spawners;
    public CraftedBaitOcean craftedBaitPool;

    public void OnTriggerEnter(Collider other)
    {
        ReagentBottle bottle = other.gameObject.GetComponent<ReagentBottle>();
        if (bottle != null && !other.isTrigger)
        {            
            if (Networking.GetOwner(other.gameObject).isLocal) {
                TryActivate(bottle);
            }
        }
    }

    public void TryActivate(ReagentBottle bottle) {
        if (!coolingDown && bottle.liquid != null && bottle.liquid.bait != null && bottle.fillLevel >= 1f) {
            indicator.SetValid();
            if (Networking.GetOwner(bottle.gameObject).isLocal) { 
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimation");
                CraftedBaitPool pool = craftedBaitPool.GetPoolByBait(bottle.liquid.bait);
                int nBaits = Random.Range(bottle.liquid.bait.craftAmount - bottle.liquid.bait.craftVariance, bottle.liquid.bait.craftAmount + bottle.liquid.bait.craftVariance);
                for (int i = 0; i < nBaits; i++) {
                    GameObject spawnedBait = pool.pool.TryToSpawn();
                    Networking.SetOwner(Networking.LocalPlayer, spawnedBait);
                    spawnedBait.transform.SetPositionAndRotation(spawners[i].position, spawners[i].rotation);
                }

                Debug.LogFormat("{0}: Spawned {1} {2}", name, nBaits, bottle.liquid.bait.name);
                
                bottle.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Despawn");
                bottle.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Empty");
                
            }
        }
        else {
            indicator.SetInvalid();
            Debug.LogFormat("{0}: Invalid bottle type, cooldown, or insufficient fill", name);
        }
        TriggerCooldown();
    }

    public void PlayAnimation() {
        if (animator != null) {
            animator.Play("Base Layer.StoveWin");
        }
    }

    public void TriggerCooldown() {
        if (!coolingDown) {
            coolingDown = true;
            SendCustomEventDelayedSeconds(nameof(EndCooldown), cooldownTime);
        }
    }

    public void EndCooldown() {
        coolingDown = false;
    }
}
