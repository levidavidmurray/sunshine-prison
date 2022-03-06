using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ClippingPlane : MonoBehaviour
{
    // material we pass the values to
    public float cutoutSize = 0.11f;
    public float falloffSize = 0.3f;
    public Material[] materials;
    public float sphereCastRadius = 2f;
    public LayerMask clippingMask;
    public Transform sphereCastVisualizer;

    void Update() {
        // pass vector to shader
        Vector3 pos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        
        float distToCamera = Mathf.Abs(Vector3.Distance(cameraPos, pos));
        Vector3 dirToCamera = (cameraPos - pos).normalized;
        
        sphereCastVisualizer.localScale = Vector3.one * sphereCastRadius;

        RaycastHit hit;
        
        // Both sphere & ray cast due to sphere cast not working when standing close to wall
        bool sphereDidHit = Physics.SphereCast(
            pos,
            sphereCastRadius,
            dirToCamera,
            out hit,
            distToCamera, 
            clippingMask
        );
        bool rayDidHit = Physics.Raycast(pos, dirToCamera, out hit, distToCamera, clippingMask);
        
        if (sphereDidHit || rayDidHit) {
            EnableClipping(hit.point);
        } else {
            DisableClipping();
        }
    }

    void EnableClipping(Vector3 hitPoint) {
        Vector3 pos = transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        
        foreach (Material mat in materials) {
            if (!mat) continue;
            
            mat.SetVector("_CutoutPos", screenPos/CameraSize());
            mat.SetFloat("_FalloffSize", falloffSize);
            mat.SetFloat("_CutoutSize", cutoutSize);
            sphereCastVisualizer.position = hitPoint;
        }
    }

    void DisableClipping() {
        foreach (Material mat in materials) {
            if (!mat) continue;
            
            mat.SetFloat("_CutoutSize", 0);
            sphereCastVisualizer.localPosition = Vector3.zero;
        }
        
    }
    
    Vector2 CameraSize() {
        return new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
    }
}
