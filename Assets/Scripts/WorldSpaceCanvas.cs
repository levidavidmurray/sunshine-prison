using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvas : MonoBehaviour {

    public Transform parentPoint;

    public bool followX = true;
    public bool followY = false;
    public bool followZ = true;
    
    private void Update() {
        transform.rotation = Camera.main.transform.rotation;

        if (parentPoint) {
            Vector3 newPoint = parentPoint.position;

            if (!followX) {
                newPoint.x = transform.position.x;
            }

            if (!followY) {
                newPoint.y = transform.position.y;
            }

            if (!followZ) {
                newPoint.z = transform.position.z;
            }
            
            transform.position = newPoint;
        }
    }
}
