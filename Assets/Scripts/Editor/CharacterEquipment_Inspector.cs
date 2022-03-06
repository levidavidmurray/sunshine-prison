using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

[CustomEditor(typeof(CharacterEquipment))]
public class CharacterEquipment_Inspector : Editor {

    public VisualElement rootVisualElement;
    public VisualTreeAsset m_inspectorXML;
    public VisualTreeAsset m_ItemRowTemplate;

    public override VisualElement CreateInspectorGUI() {
        rootVisualElement = new VisualElement();
        
        rootVisualElement.Add(new Label("This is a custom inspector"));

        // Load and clone a visual tree from UXML
        m_inspectorXML.CloneTree(rootVisualElement);

        // Attach default inspector to foldout control
        VisualElement inspectorFoldout = rootVisualElement.Q("Default_Inspector");
        InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);
        
        GenerateList();

        return rootVisualElement;
    }

    public void GenerateList() {
        Func<VisualElement> makeItem = () => m_ItemRowTemplate.CloneTree();
        Action<VisualElement, int> bindItem = (e, i) => {
            // e.Q<VisualElement>("Icon").style.backgroundImage = 
            // e.Q<Label>("Name").text = 
        };

        VisualElement equipRow = rootVisualElement.Q("EquipmentRow");

        SerializedProperty equipProperty = serializedObject.FindProperty("currentEquipment");
        string[] equipSlotNames = System.Enum.GetNames(typeof(Equipment.EquipmentSlot));

        for (int i = 0; i < equipProperty.arraySize; i++) {
            VisualElement itemRow = m_ItemRowTemplate.CloneTree();
            itemRow.Q<Label>("Name").text = equipSlotNames[i];
            equipRow.Add(itemRow);
        }
        
    }
    
}
