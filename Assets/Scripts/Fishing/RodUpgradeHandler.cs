
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RodUpgradeHandler : UdonSharpBehaviour
{
    public RodUpgrade[] upgrades;
    public FishingPole fishingPole;
    public SkinnedMeshRenderer meshRenderer;
    public Material material;
    [UdonSynced] public Color color = Color.white;

    [UdonSynced, FieldChangeCallback(nameof(UpgradeLevel))]
    private int _upgradeLevel;
    public int UpgradeLevel
    {
        set
        {
            if (value > _upgradeLevel) {
                _upgradeLevel = value;
            }
            SetRodUpgrade();
        }
        get => _upgradeLevel;
    }

    public void Start() {
        if (meshRenderer != null) { 
            material = meshRenderer.material;
        }
    }

    public void SetRodUpgrade() {
        foreach (RodUpgrade upgrade in upgrades) {
            if (upgrade.level == UpgradeLevel) {
                fishingPole.rodLevelParams = upgrade;
                if (upgrade.mesh != null) {
                    meshRenderer.sharedMesh = upgrade.mesh;
                }
                if (upgrade.material != null) {
                    meshRenderer.material = upgrade.material;
                }

                material = meshRenderer.material;
                material.color = color;

                break;
            }
        }
    }

    public void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.name.Contains("Rod Upgrade")) {
            if (Networking.GetOwner(coll.gameObject).isLocal && Networking.GetOwner(fishingPole.gameObject).isLocal) {
                RodUpgradePotion upgradePotion = coll.GetComponent<RodUpgradePotion>();
                
                if (upgradePotion != null && upgradePotion.rodUpgrade != null) {
                    if (upgradePotion.rodUpgrade.level > UpgradeLevel) {
                        UpgradeLevel = upgradePotion.rodUpgrade.level;
                        upgradePotion.Shatter();
                    }
                }
            }
        }
    }
}
