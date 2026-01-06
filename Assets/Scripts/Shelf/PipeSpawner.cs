
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class PipeSpawner : UdonSharpBehaviour
{
    public Transform targetTransform;
    public VRCPickup pickup;
    public LineRenderer lineRenderer;
    public bool isRayCastActive = false;
    
    
    public float rayCastDistance = 10f;
    public LayerMask rayCastLayerMask;
    public float rayCastOffset = 0.5f;
    public Material material;
    public Material lineMaterial;
    public MeshRenderer meshRenderer;

    public void Start()
    {
        material = meshRenderer.materials[0];
        lineMaterial = lineRenderer.materials[0];
        lineRenderer.enabled = false;
        OnPlayerRestored(Networking.LocalPlayer);
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        Random.InitState(Networking.GetOwner(gameObject).displayName.GetHashCode());
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        material.SetColor("_Color", randomColor);
        Random.InitState(Time.frameCount);
    }

    public override void OnPickupUseDown()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + (transform.up * rayCastOffset);
        if (Physics.Raycast(rayOrigin, transform.up, out hit, rayCastDistance, rayCastLayerMask))
        {
            if (hit.transform != null && hit.transform.gameObject != null && hit.transform.gameObject.layer == 11) {
                var objects = Networking.GetPlayerObjects(Networking.LocalPlayer);
                for (int i = 0; i < objects.Length; i++)
                {
                    if (!Utilities.IsValid(objects[i])) continue;
                    PersonalFishPipe personalPipe = objects[i].GetComponentInChildren<PersonalFishPipe>();
                    if (Utilities.IsValid(personalPipe)) {
                        personalPipe.transform.position = hit.point;
                        personalPipe.Spawn();
                    }
                }
                pickup.Drop();
            }
        }
    }

    public override void OnPickup()
    {
        isRayCastActive = true;
        lineRenderer.enabled = true;
    }

    public override void OnDrop()
    {
        isRayCastActive = false;
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
        lineRenderer.enabled = false;
    }


    public void Update()
    {
        if (isRayCastActive)
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + (transform.up * rayCastOffset);
            if (Physics.Raycast(rayOrigin, transform.up, out hit, rayCastDistance, rayCastLayerMask))
            {
                if (hit.transform == null) return;
                if (hit.transform.gameObject.layer == 11) {
                    lineMaterial.SetColor("_Color", Color.green);
                }
                else {
                    lineMaterial.SetColor("_Color", Color.red);
                }
            }
            else {
                lineMaterial.SetColor("_Color", Color.red);
            }
        }
    }
}
