using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
public class HierarchyExtension
{
    /// <summary>
    /// Initializer <see cref="HierarchyExtension"/> class.
    /// </summary>
    static HierarchyExtension()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchWindowOnGui;
    }

    /// <summary>
    /// Editor delegate callback
    /// </summary>
    /// <param name="instanceID">İnstance id.</param>
    /// <param name="selectionRect">Selection rect.</param>
    static void HierarchWindowOnGui(int instanceID, Rect selectionRect)
    {
        // make rectangle
        Rect r = new Rect(selectionRect);
        r.x = 35;
        r.width = 18;
        
        // get objects
        Object o = EditorUtility.InstanceIDToObject(instanceID);
        GameObject g = o as GameObject;
        
        // drag toggle gui
        if (g != null)
        {
            var prev = g.activeSelf;
            g.SetActive(GUI.Toggle(r, g.activeSelf, string.Empty));

            if (g.activeSelf != prev)
            {
                EditorUtility.SetDirty(g);
            }
        }
    }
}