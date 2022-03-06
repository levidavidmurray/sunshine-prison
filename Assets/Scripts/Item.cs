using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : ScriptableObject {

    new public string name = "New Item";

    // public Transform owner;
    
    void Start() {
        // SkinnedMeshRenderer itemSmr = GetComponent<SkinnedMeshRenderer>();
        // SkinnedMeshRenderer ownerSmr = owner.GetComponent<SkinnedMeshRenderer>();
        // itemSmr.bones = ownerSmr.bones;
    }
    
}
