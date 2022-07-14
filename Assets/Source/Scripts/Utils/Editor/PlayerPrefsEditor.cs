using System.IO;
using UnityEditor;
using UnityEngine;

public static class PlayerPrefsEditor
{
    [MenuItem("Utils/Clear All Prefs", false, 1)]
    private static void ClearAllPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}