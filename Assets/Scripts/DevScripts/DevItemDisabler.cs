
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class DevItemDisabler : UdonSharpBehaviour
{
    public GameObject[] devObjects;

    [FieldChangeCallback(nameof(DevObjectsEnabled))]
    private bool _devObjectsEnabled;
    public bool DevObjectsEnabled {
        set {
            _devObjectsEnabled = value;
            ToggleDevObjects();
        }

        get => _devObjectsEnabled;
    }

    public void ToggleDevObjects() {
        foreach (GameObject devObject in devObjects) {
            devObject.SetActive(DevObjectsEnabled);
        }
    }

    [ContextMenu("Toggle Dev Objects")]
    public void ToggleDev() {
        DevObjectsEnabled = !DevObjectsEnabled;
    }

}
