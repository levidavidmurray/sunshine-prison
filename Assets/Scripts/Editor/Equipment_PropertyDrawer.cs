using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;

// [CustomPropertyDrawer(typeof(Equipment))]
// public class Equipment_PropertyDrawer : PropertyDrawer
// {
//     public override VisualElement CreatePropertyGUI(SerializedProperty property) {
//         var container = new VisualElement();
//         
//         // Create drawer UI using C#
//         var popup = new PopupWindow();
//         popup.text = "Equipment Item";
//         popup.Add(new PropertyField(property.FindPropertyRelative("name"), "name"));
//         popup.Add(new PropertyField(property.FindPropertyRelative("mesh"), "mesh"));
//         container.Add(popup);
//
//         return container;
//     }
// }
