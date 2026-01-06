using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MakeCollidersTriggers : MonoBehaviour
{
    [ContextMenu("Make Colliders Triggers")]
    public void MakeTriggers()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger) continue;
            collider.isTrigger = true;
        }
    }
}
