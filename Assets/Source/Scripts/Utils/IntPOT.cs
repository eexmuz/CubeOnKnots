using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct IntPOT
{
    public int POT;
    public int Number;
    
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(IntPOT))]
    public class IntPOTDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pot = property.FindPropertyRelative(nameof(POT));
            var value = property.FindPropertyRelative(nameof(Number));
            
            
            EditorGUI.BeginProperty(position, label, property);
            {
                var v2 = EditorGUI.Vector2IntField(position, label, new Vector2Int(pot.intValue, value.intValue));
                var potValue = v2.x;
                var valueValue = v2.y;

                if (potValue != pot.intValue)
                {
                    potValue = Mathf.Max(potValue, 0);
                    valueValue = 1 << potValue;
                }

                if (valueValue != value.intValue)
                {
                    valueValue = Mathf.ClosestPowerOfTwo(valueValue);
                    potValue = 0;
                    int tmp = valueValue;
                    while (tmp > 1)
                    {
                        tmp >>= 1;
                        potValue++;
                    }
                }

                pot.intValue = potValue;
                value.intValue = valueValue;
            }
            EditorGUI.EndProperty();
        }
    }

#endif
}