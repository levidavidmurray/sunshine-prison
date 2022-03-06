using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKTargeter : MonoBehaviour {
    public IKTarget[] targets;
    
    private void OnTriggerStay(Collider other) {
        if (!other.CompareTag("Player")) return;
        foreach (IKTarget target in targets) {
            target.Join();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;
        foreach (IKTarget target in targets) {
            target.Break();
        }
    }
}
