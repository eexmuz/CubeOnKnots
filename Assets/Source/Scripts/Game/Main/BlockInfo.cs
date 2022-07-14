using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BlockInfo
{
    public int PowerOfTwo;
    public bool Movable;
    public bool Mergeable;
    
#if UNITY_EDITOR && false

    [CustomPropertyDrawer(typeof(BlockInfo))]
    public class BlockInfoDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pot = property.FindPropertyRelative(nameof(PowerOfTwo));
            var movable = property.FindPropertyRelative(nameof(Movable));
            var mergeable = property.FindPropertyRelative(nameof(Mergeable));
            
            
            EditorGUI.BeginProperty(position, label, property);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    var potValue = EditorGUI.IntField(position, label, pot.intValue);
                    var movableValue = EditorGUI.Toggle(position, label, movable.boolValue);
                    var mergeableValue = EditorGUI.Toggle(position, label, mergeable.boolValue);
                    // var potValue = EditorGUILayout.IntField(label, pot.intValue);
                    // var movableValue = EditorGUILayout.Toggle(label, movable.boolValue);
                    // var mergeableValue = EditorGUILayout.Toggle(label, mergeable.boolValue);

                    pot.intValue = potValue;
                    movable.boolValue = movableValue;
                    mergeable.boolValue = mergeableValue;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndProperty();
        }
    }

#endif
}