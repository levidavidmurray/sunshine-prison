using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTarget : MonoBehaviour {
    public Transform rigTarget;

    private Vector3 _defaultPosition;

    void Start() {
        _defaultPosition = rigTarget.position;
    }

    public void Join() {
        rigTarget.position = transform.position;
    }

    public void Break() {
        rigTarget.position = _defaultPosition;
    }
    
}
