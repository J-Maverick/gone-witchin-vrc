using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FishNameList : MonoBehaviour
{
    public String fishes;
    public int count;

    [ContextMenu("Create Fish Name List")]
    public void CreateFishNameList()
    {
        fishes = "";
        count = 0;
        foreach (Transform child in transform)
        {
            fishes += child.name + "\n";
            count++;
        }
    }
}
