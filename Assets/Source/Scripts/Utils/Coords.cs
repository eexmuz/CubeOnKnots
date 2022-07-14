using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct Coords
{
    public int x;
    public int y;

    public int Index(int width) => y * width + x;

    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is Coords other)
        {
            return other.x == x && other.y == y;
        }
        
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        int y = this.y, mul = 1;
        while (y > 0)
        {
            y /= 10;
            mul *= 10;
        }

        long result = x * mul + y; 

        return result.GetHashCode();
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }

    public static int Index(int x, int y, int width) => y * width + x;
    
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(Coords))]
    public class CoordsDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pX = property.FindPropertyRelative(nameof(x));
            var pY = property.FindPropertyRelative(nameof(y));
            
            
            EditorGUI.BeginProperty(position, label, property);
            {
                var val = EditorGUI.Vector2IntField(position, label, new Vector2Int(pX.intValue, pY.intValue));
                pX.intValue = val.x;
                pY.intValue = val.y;
            }
            EditorGUI.EndProperty();
        }
    }

#endif
}
