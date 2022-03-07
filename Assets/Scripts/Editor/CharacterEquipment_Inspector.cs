using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(CharacterEquipment))]
public class CharacterEquipment_Inspector : Editor {

    public VisualElement rootVisualElement;
    public VisualTreeAsset m_inspectorXML;
    public VisualTreeAsset m_ItemRowTemplate;
    public ListView m_equipListView;

    private SerializedProperty m_equipment;
    private CharacterEquipment m_characterEquipment;

    public override VisualElement CreateInspectorGUI() {
        rootVisualElement = new VisualElement();
        
        // Load and clone a visual tree from UXML
        m_inspectorXML.CloneTree(rootVisualElement);

        // Attach default inspector to foldout control
        VisualElement inspectorFoldout = rootVisualElement.Q("Default_Inspector");
        InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

        m_characterEquipment = (CharacterEquipment)serializedObject.targetObject;
        
        GenerateList();

        // rootVisualElement.Q<Button>("Button_Refresh").clicked += () => CreateInspectorGUI();

        return rootVisualElement;
    }

    public void GenerateList() {
        Debug.Log("Generating List...");

        VisualElement equipRow = rootVisualElement.Q("EquipmentRow");

        m_equipment = serializedObject.FindProperty("currentEquipment");
        string[] equipSlotNames = System.Enum.GetNames(typeof(Equipment.EquipmentSlot));

        Func<VisualElement> makeItem = () => {
            VisualElement itemRow = m_ItemRowTemplate.CloneTree();
            RegisterItemRowDropArea(itemRow);
            return itemRow;
        };
        Action<VisualElement, int> bindItem = (e, i) => {
            e.Q<Label>("SlotType").text = equipSlotNames[i];

            Equipment item = (Equipment)m_equipment.GetArrayElementAtIndex(i).objectReferenceValue;
            e.Q<Label>("ItemName").text = item ? item.name : "None";

            if (item) {
                m_characterEquipment.Equip(item);
            }
            else {
                m_characterEquipment.Unequip(i);
            }
            
        };
        
        m_equipListView = new ListView(new List<Equipment>(), 35f, makeItem, bindItem);
        m_equipListView.showBoundCollectionSize = false;
        m_equipListView.bindingPath = "currentEquipment";
        m_equipListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        m_equipListView.style.flexGrow = 1;

        m_equipListView.onSelectionChange += ListView_onSelectionChange;
        m_equipListView.RegisterCallback<KeyDownEvent>(ListView_KeyDownHandler);

        rootVisualElement.Add(m_equipListView);
        rootVisualElement.Bind(serializedObject);
        
        //
        // for (int i = 0; i < equipSlotNames.Length; i++) {
        //     SerializedProperty slotItem = m_equipment.GetArrayElementAtIndex(i);
        //     VisualElement itemRow = m_ItemRowTemplate.CloneTree();
        //     itemRow.name = $"ItemRow_{i}";
        //     itemRow.Unbind();
        //
        //     if (slotItem.objectReferenceValue != null) {
        //         itemRow.Bind(new SerializedObject(slotItem.objectReferenceValue));
        //     }
        //     
        //     itemRow.Q<Label>("SlotType").text = equipSlotNames[i];
        //     
        //     RegisterItemRowDropArea(itemRow, i);
        //     equipRow.Add(itemRow);
        // }
    }

    void ListView_onSelectionChange(IEnumerable<object> selectedItems) {
    }

    void ListView_KeyDownHandler(KeyDownEvent evt) {
        Debug.Log($"[KeyDownHandler]: keycode: {evt.keyCode}");
        if (evt.keyCode != KeyCode.Backspace && evt.keyCode != KeyCode.Delete) return;
        
        // Delete selected items
        foreach (int i in m_equipListView.selectedIndices) {
            RemoveEquipmentSlotItem(i);
        }
    }

    private void RegisterItemRowDropArea(VisualElement itemRow) {
        itemRow.RegisterCallback<DragEnterEvent>(OnDragEnterEvent);
        itemRow.RegisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
        itemRow.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        itemRow.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
        itemRow.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);
    }

    bool HasEquipmentAsset(string[] paths) {
        foreach (string path in DragAndDrop.paths) {
            if (AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(Equipment)) {
                return true;
            }
        }

        return false;
    }

    void OnDragEnterEvent(DragEnterEvent e) {
        Debug.Log("DRAG Enter");
    }
    
    void OnDragLeaveEvent(DragLeaveEvent e) {
        Debug.Log("DRAG Leave");
    }
    
    void OnDragUpdatedEvent(DragUpdatedEvent e) {
        if (!HasEquipmentAsset(DragAndDrop.paths)) return;

        Debug.Log("ACCEPTING!");
        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
    }
    
    void OnDragPerformEvent(DragPerformEvent e) {
        Debug.Log("DRAG Perform");
        DragAndDrop.AcceptDrag();

        if (!HasEquipmentAsset(DragAndDrop.paths)) return;

        serializedObject.Update();

        foreach (string path in DragAndDrop.paths) {
            Equipment newItem = AssetDatabase.LoadAssetAtPath<Equipment>(path);
            if (!newItem) continue;
            UpdateEquipmentSlot(newItem);
            Debug.Log($"Dragged {newItem.name}");
        }
    }
    
    void OnDragExitedEvent(DragExitedEvent e) {
        Debug.Log("DRAG Exited");
    }

    void UpdateEquipmentSlot(Equipment newItem) {
        if (!newItem) return;
        // m_characterEquipment.Equip(newItem);
        m_equipment.GetArrayElementAtIndex((int)newItem.equipSlot).objectReferenceValue = newItem;
        serializedObject.ApplyModifiedProperties();
        m_equipListView.RefreshItem((int)newItem.equipSlot);
    }

    void RemoveEquipmentSlotItem(int itemSlot) {
        m_characterEquipment.Unequip(itemSlot);
        m_equipment.GetArrayElementAtIndex(itemSlot).objectReferenceValue = null;
        serializedObject.ApplyModifiedProperties();
        m_equipListView.RefreshItem(itemSlot);
    }
    
    private void PointerEnterHandler(PointerEnterEvent evt, int slotIndex) {
        // DragAndDrop.
    }
    
}
