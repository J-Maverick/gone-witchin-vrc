using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTreeCollision : MonoBehaviour
{
    public void Awake()
    {
        ResetCollision();
    }

    public void ResetCollision()
    {
        if (GetComponent<TerrainCollider>() == null) return;
        GetComponent<TerrainCollider>().enabled = false;
        GetComponent<TerrainCollider>().enabled = true;
    }
}
