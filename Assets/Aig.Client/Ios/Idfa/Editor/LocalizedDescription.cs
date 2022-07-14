using System;
using UnityEditor;
using UnityEngine;

namespace Aig.Client.Ios.Idfa.Editor
{
    [Serializable]
    public class LocalizedDescription
    {
        public string langCode;
        public string langName;
        public string description;

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(langCode) || string.IsNullOrWhiteSpace(langCode) ||
                   string.IsNullOrEmpty(description) || string.IsNullOrWhiteSpace(description);
        }
    }

    [CustomPropertyDrawer(typeof(LocalizedDescription))]
    public class LocalizedDescriptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(2);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            var langCodeProperty = property.FindPropertyRelative("langCode");
            var langNameProperty = property.FindPropertyRelative("langName");
            var descriptionTextProperty = property.FindPropertyRelative("description");

            EditorGUILayout.LabelField($"{langNameProperty.stringValue} ({langCodeProperty.stringValue.ToUpper()})");

            EditorGUI.indentLevel++;

            if (string.IsNullOrEmpty(descriptionTextProperty.stringValue) == false)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(descriptionTextProperty.stringValue);
            }

            EditorGUI.indentLevel = indent;

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
        }
    }
}