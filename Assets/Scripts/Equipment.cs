using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item {

    public enum EquipmentSlot {
        Hat,
        Hair,
        FacialHair,
        Torso,
        Legs,
        Feet,
        Weapon,
    }
    
    public EquipmentSlot equipSlot;
    public SkinnedMeshRenderer mesh;
}
