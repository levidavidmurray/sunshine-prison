using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
[ExecuteAlways]
public class CharacterEquipment : MonoBehaviour {

    public SkinnedMeshRenderer targetMesh;
    public Equipment[] currentEquipment = new Equipment[System.Enum.GetNames(typeof(Equipment.EquipmentSlot)).Length];
    private SkinnedMeshRenderer[] currentMeshes = new SkinnedMeshRenderer[System.Enum.GetNames(typeof(Equipment.EquipmentSlot)).Length];
    
    public void Equip(Equipment newItem) {
        int slotIndex = (int)newItem.equipSlot;
        currentEquipment[slotIndex] = newItem;
        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.mesh);
        newMesh.transform.parent = targetMesh.transform;

        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;
        currentMeshes[slotIndex] = newMesh;
    }

    public void Unequip(int slotIndex) {
        if (!currentEquipment[slotIndex]) return;

        if (currentMeshes[slotIndex]) {
            Destroy(currentMeshes[slotIndex].gameObject);
            
        }

        currentEquipment[slotIndex] = null;
    }

}
