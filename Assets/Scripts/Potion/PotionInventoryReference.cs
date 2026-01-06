
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class PotionInventoryReference : UdonSharpBehaviour
{
    public Transform spawnTarget;
    public Transform sharedInventoryUIParent;
    public Transform personalInventoryUIParent;
    public TMP_Text sharedInventoryText;
    public TMP_Text personalInventoryText;
    public PotionInventoryStorage potionInventoryStorage;
    public FishList fishList;
    public BaitInventoryUI baitInventoryUI;

}
