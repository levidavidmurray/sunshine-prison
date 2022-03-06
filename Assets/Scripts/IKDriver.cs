using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKDriver : MonoBehaviour {
    public Rig armRig;
    public Rig headRig;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("IKTargeter")) return;
        
        
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("IKTargeter")) return;
        
    }
}
