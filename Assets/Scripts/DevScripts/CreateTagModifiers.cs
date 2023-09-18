using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class CreateTagModifiers : MonoBehaviour
{
    [ContextMenu("Create Tag Modifiers")]
    public void MakeMods() {

        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        List<ModType> modTypes = new List<ModType>();
        // Normal Boosts
        modTypes.Add(new ModType("Boost2x", "Boost_2x_", "", 2f, false, false, false));
        modTypes.Add(new ModType("Boost5x", "Boost_5x_", "", 5f, false, false, false));
        modTypes.Add(new ModType("Boost10x", "Boost_10x_", "", 10f, false, false, false));
        
        modTypes.Add(new ModType("Reduce2x", "Reduce_2x_", "", 1f / 2f, false, false, false));
        modTypes.Add(new ModType("Reduce5x", "Reduce_5x_", "", 1f / 5f, false, false, false));
        modTypes.Add(new ModType("Reduce10x", "Reduce_10x_", "", 1f / 10f, false, false, false));

        modTypes.Add(new ModType("Inverse_Boost2x", "Inverse_Boost_2x_", "", 2f, true, false, false));
        modTypes.Add(new ModType("Inverse_Boost5x", "Inverse_Boost_5x_", "", 5f, true, false, false));
        modTypes.Add(new ModType("Inverse_Boost10x", "Inverse_Boost_10x_", "", 10f, true, false, false));
        
        modTypes.Add(new ModType("Inverse_Reduce2x", "Inverse_Reduce_2x_", "", 1f / 2f, true, false, false));
        modTypes.Add(new ModType("Inverse_Reduce5x", "Inverse_Reduce_5x_", "", 1f / 5f, true, false, false));
        modTypes.Add(new ModType("Inverse_Reduce10x", "Inverse_Reduce_10x_", "", 1f / 10f, true, false, false));

        // Lake only boosts
        modTypes.Add(new ModType("Boost2x_Lake_Only", "Boost_2x_", "_Lake_Only", 2f, false, true, false));
        modTypes.Add(new ModType("Boost5x_Lake_Only", "Boost_5x_", "_Lake_Only", 5f, false, true, false));
        modTypes.Add(new ModType("Boost10x_Lake_Only", "Boost_10x_", "_Lake_Only", 10f, false, true, false));
        
        modTypes.Add(new ModType("Reduce2x_Lake_Only", "Reduce_2x_", "_Lake_Only", 1f / 2f, false, true, false));
        modTypes.Add(new ModType("Reduce5x_Lake_Only", "Reduce_5x_", "_Lake_Only", 1f / 5f, false, true, false));
        modTypes.Add(new ModType("Reduce10x_Lake_Only", "Reduce_10x_", "_Lake_Only", 1f / 10f, false, true, false));

        modTypes.Add(new ModType("Inverse_Boost2x_Lake_Only", "Inverse_Boost_2x_", "_Lake_Only", 2f, true, true, false));
        modTypes.Add(new ModType("Inverse_Boost5x_Lake_Only", "Inverse_Boost_5x_", "_Lake_Only", 5f, true, true, false));
        modTypes.Add(new ModType("Inverse_Boost10x_Lake_Only", "Inverse_Boost_10x_", "_Lake_Only", 10f, true, true, false));
        
        modTypes.Add(new ModType("Inverse_Reduce2x_Lake_Only", "Inverse_Reduce_2x_", "_Lake_Only", 1f / 2f, true, true, false));
        modTypes.Add(new ModType("Inverse_Reduce5x_Lake_Only", "Inverse_Reduce_5x_", "_Lake_Only", 1f / 5f, true, true, false));
        modTypes.Add(new ModType("Inverse_Reduce10x_Lake_Only", "Inverse_Reduce_10x_", "_Lake_Only", 1f / 10f, true, true, false));

        // Cave only boosts
        modTypes.Add(new ModType("Boost2x_Cave_Only", "Boost_2x_", "_Cave_Only", 2f, false, false, true));
        modTypes.Add(new ModType("Boost5x_Cave_Only", "Boost_5x_", "_Cave_Only", 5f, false, false, true));
        modTypes.Add(new ModType("Boost10x_Cave_Only", "Boost_10x_", "_Cave_Only", 10f, false, false, true));
        
        modTypes.Add(new ModType("Reduce2x_Cave_Only", "Reduce_2x_", "_Cave_Only", 1f / 2f, false, false, true));
        modTypes.Add(new ModType("Reduce5x_Cave_Only", "Reduce_5x_", "_Cave_Only", 1f / 5f, false, false, true));
        modTypes.Add(new ModType("Reduce10x_Cave_Only", "Reduce_10x_", "_Cave_Only", 1f / 10f, false, false, true));

        modTypes.Add(new ModType("Inverse_Boost2x_Cave_Only", "Inverse_Boost_2x_", "_Cave_Only", 2f, true, false, true));
        modTypes.Add(new ModType("Inverse_Boost5x_Cave_Only", "Inverse_Boost_5x_", "_Cave_Only", 5f, true, false, true));
        modTypes.Add(new ModType("Inverse_Boost10x_Cave_Only", "Inverse_Boost_10x_", "_Cave_Only", 10f, true, false, true));
        
        modTypes.Add(new ModType("Inverse_Reduce2x_Cave_Only", "Inverse_Reduce_2x_", "_Cave_Only", 1f / 2f, true, false, true));
        modTypes.Add(new ModType("Inverse_Reduce5x_Cave_Only", "Inverse_Reduce_5x_", "_Cave_Only", 1f / 5f, true, false, true));
        modTypes.Add(new ModType("Inverse_Reduce10x_Cave_Only", "Inverse_Reduce_10x_", "_Cave_Only", 1f / 10f, true, false, true));

        

        foreach(ModType modType in modTypes) {
            GameObject folder = new GameObject(modType.folderName);
            folder.transform.parent = transform;
            foreach(FishTag fishTag in Enum.GetValues(typeof(FishTag))) {
                GameObject tagObject = new GameObject(string.Format("{0}{1}{2}", modType.prefix, fishTag.ToString(), modType.suffix));
                tagObject.transform.parent = folder.transform;
                TagModifier tagMod = tagObject.AddComponent<TagModifier>();
                tagMod.modifierTag = fishTag;
                tagMod.modifier = modType.modifier;
                tagMod.inverse = modType.inverse;

                if (modType.lakeOnly) {
                    tagMod.activeInCave = false;
                }
                else if (modType.caveOnly) {
                    tagMod.activeInLake = false;
                }
            }
        }
    }
}

public class ModType 
{
    public string folderName;
    public string prefix;
    public string suffix;
    public float modifier;
    public bool inverse;
    public bool lakeOnly;
    public bool caveOnly;

    public ModType(string folderNameIn, string prefixIn, string suffixIn, float modfifierIn, bool inverseIn, bool lakeOnlyIn=false, bool caveOnlyIn=false) {
        folderName = folderNameIn;
        prefix = prefixIn;
        suffix = suffixIn;
        modifier = modfifierIn;
        inverse = inverseIn;
        lakeOnly = lakeOnlyIn;
        caveOnly = caveOnlyIn;
    }
}
