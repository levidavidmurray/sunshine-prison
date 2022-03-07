using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelToItemPrefab_Importer : AssetPostprocessor {
    
    private static string QueuedItemSOPath;
    private static string QueuedItemPrefabPath;
    private static string QueuedItemPrefabName;
    
    void OnPostprocessModel(GameObject g) {
        Debug.Log($"Postprocess Model! {g.name} at {assetPath}");

        if (!assetPath.Contains("Assets/Items/Meshes")) return;

        QueuedItemSOPath = null;
        QueuedItemPrefabPath = null;
        QueuedItemPrefabName = null;
        
        SkinnedMeshRenderer[] mfs = g.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        foreach (SkinnedMeshRenderer mf in mfs) {
            Debug.Log($"Saving mesh: {mf.name}");
            var meshPath = $"Assets/Items/Meshes/{mf.name}_Mesh.asset";
            AssetDatabase.DeleteAsset(meshPath);
            AssetDatabase.CreateAsset(mf.sharedMesh, meshPath);

            foreach (Material mat in mf.sharedMaterials) {
                var matPath = $"Assets/Items/Meshes/{mf.name}_{mat.name}.mat";
                AssetDatabase.DeleteAsset(matPath);
                AssetDatabase.CreateAsset(mat, matPath);
            }
            
            var prefabPath = $"Assets/Items/Meshes/{mf.name}.prefab";
            GameObject itemPrefab = mf.gameObject;
            itemPrefab.name = mf.name;
            PrefabUtility.SaveAsPrefabAsset(itemPrefab, prefabPath);
            
            // Prefab not available in order to set mesh on Equipment ScriptableObject, defer to OnPostprocessPrefab
            QueuedItemPrefabPath = prefabPath;
            QueuedItemPrefabName = mf.name;
        }
    }

    // Can possibly move this to OnPostprocessAllAssets
    private void OnPostprocessPrefab(GameObject g) {
        if (g.name != QueuedItemPrefabName) return;
        
        var itemPath = $"Assets/Items/{g.name}.asset";
        Equipment equipSO = AssetDatabase.LoadAssetAtPath<Equipment>(itemPath);
        
        // Create Equipment scriptable object
        if (!equipSO) {
            equipSO = ScriptableObject.CreateInstance<Equipment>();
            equipSO.mesh = g.GetComponent<SkinnedMeshRenderer>();
            equipSO.name = g.name;
            equipSO.equipSlot = GetEquipSlot(equipSO.name);
            AssetDatabase.CreateAsset(equipSO, itemPath);
        }
        else {
            equipSO.mesh = g.GetComponent<SkinnedMeshRenderer>();
            equipSO.name = g.name;
            equipSO.equipSlot = GetEquipSlot(equipSO.name);
            EditorUtility.SetDirty(equipSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        QueuedItemPrefabName = null;
        QueuedItemSOPath = itemPath;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths) {
        
        if (QueuedItemSOPath == null || QueuedItemPrefabPath == null) return;
        
        foreach (string assetPath in importedAssets) {
            if (assetPath == QueuedItemSOPath) {
                GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(QueuedItemPrefabPath);
                Equipment equipSO = AssetDatabase.LoadAssetAtPath<Equipment>(assetPath);
                equipSO.mesh = itemPrefab.GetComponent<SkinnedMeshRenderer>();
                
                EditorUtility.SetDirty(equipSO);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                QueuedItemSOPath = null;
                QueuedItemPrefabPath = null;
            }
        }
    }

    Equipment.EquipmentSlot GetEquipSlot(string name) {
        var equipPrefix = name.Substring(0, name.IndexOf("_"));
        return System.Enum.Parse<Equipment.EquipmentSlot>(equipPrefix);
    }
}
