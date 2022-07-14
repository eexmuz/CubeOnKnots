using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Settings.Editor
{
    [CustomEditor(typeof(UiSystemSettingsAsset))]
    public class UiInitializationDataEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/Settings/UI System Settings")]
        public static void CreateUiSettings()
        {
            var uiSettings = CreateInstance<UiSystemSettingsAsset>();

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path)) path = "Assets";

            var projectDir = new DirectoryInfo(Application.dataPath).Parent;
            if (projectDir != null)
            {
                var fullPath = Path.Combine(projectDir.FullName, path);

                if ((File.GetAttributes(fullPath) & FileAttributes.Directory) != FileAttributes.Directory)
                    path = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
            }

            path = Path.Combine(path, "UISettings.asset");

            AssetDatabase.CreateAsset(uiSettings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = uiSettings;
        }

        private void OnEnable()
        {
            var data = serializedObject.FindProperty("data");

            _names = data.FindPropertyRelative("names");
            _prefabs = data.FindPropertyRelative("prefabs");
            _shroudPrefab = data.FindPropertyRelative("shroudPrefab");
            _uiLockerPrefab = data.FindPropertyRelative("uiLockerPrefab");

            var enumValuesArray = Enum.GetValues(typeof(ViewName));
            _enumValues = new ViewName[enumValuesArray.Length];

            for (var i = 0; i < enumValuesArray.Length; i++) _enumValues[i] = (ViewName) enumValuesArray.GetValue(i);

            _viewNames = _names.enumNames;
            _addedViewName = _enumValues[0];

            UpdateNamesHashSet();
        }

        public override void OnInspectorGUI()
        {
            _addControlsExpanded = EditorGUILayout.Foldout(_addControlsExpanded, "Add new dialog");
            var isItemAdded = false;

            if (_addControlsExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                _addedViewName = (ViewName) EditorGUILayout.EnumPopup(_addedViewName);
                _addedPrefab = EditorGUILayout.ObjectField(_addedPrefab, typeof(GameObject), false);

                var newItemErrors = new List<string>();

                if (_addedDialogsNames.Contains(_addedViewName))
                    newItemErrors.Add(string.Format("* Dialog {0} is already linked. Select another type.",
                        _addedViewName));
                if (_addedPrefab == null) newItemErrors.Add("* Prefab field should not be empty!");

                PushGuiEnabledState(newItemErrors.Count == 0);
                if (GUILayout.Button("Add"))
                {
                    var index = _names.arraySize;

                    _names.InsertArrayElementAtIndex(index);
                    _names.GetArrayElementAtIndex(index).enumValueIndex =
                        Array.IndexOf(_viewNames, _addedViewName.ToString());

                    _prefabs.InsertArrayElementAtIndex(index);
                    _prefabs.GetArrayElementAtIndex(index).objectReferenceValue = _addedPrefab;

                    _addedDialogsNames.Add(_addedViewName);
                    isItemAdded = true;
                }

                PopGuiEnabledState();
                EditorGUILayout.EndHorizontal();

                if (newItemErrors.Count > 0)
                    foreach (var errMsg in newItemErrors)
                        DrawErrorLabel(errMsg);

                if (GUILayout.Button("Add missing Views"))
                    for (var i = 0; i < _enumValues.Length; i++)
                    {
                        var viewName = _enumValues[i];
                        if (!_addedDialogsNames.Contains(viewName))
                        {
                            var index = _names.arraySize;
                            _names.InsertArrayElementAtIndex(index);
                            _names.GetArrayElementAtIndex(index).enumValueIndex = i;

                            _prefabs.InsertArrayElementAtIndex(index);
                            isItemAdded = true;
                        }
                    }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _dialogsExpanded = EditorGUILayout.Foldout(_dialogsExpanded, "Dialogs Links");

            EditorGUI.BeginChangeCheck();
            if (_dialogsExpanded)
            {
                if (_names.arraySize > 0)
                {
                    var indexToDelete = -1;
                    for (var i = 0; i < _names.arraySize; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(string.Format("{0}.", i), GUILayout.Width(20));
                        var currentItem = _names.GetArrayElementAtIndex(i);

                        if (currentItem.enumValueIndex < 0)
                        {
                            Debug.LogErrorFormat("Wrong enum value ({0}) for item #{1}.", currentItem.enumValueIndex,
                                i);
                            currentItem.enumValueIndex = 0;
                        }

                        var currentName = (ViewName) _enumValues.GetValue(currentItem.enumValueIndex);
                        var newName = (ViewName) EditorGUILayout.EnumPopup(currentName);
                        if (newName != currentName)
                            _names.GetArrayElementAtIndex(i).enumValueIndex = Array.IndexOf(_enumValues, newName);
                        _prefabs.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField(
                            _prefabs.GetArrayElementAtIndex(i).objectReferenceValue, typeof(GameObject), false);
                        if (GUILayout.Button("X")) indexToDelete = i;
                        EditorGUILayout.EndHorizontal();
                    }

                    if (indexToDelete >= 0)
                    {
                        var enumValueIndexToDelete = _names.GetArrayElementAtIndex(indexToDelete).enumValueIndex;
                        if (enumValueIndexToDelete >= 0 && enumValueIndexToDelete < _enumValues.Length)
                        {
                            var nameToDelete = _enumValues[enumValueIndexToDelete];
                            _addedDialogsNames.Remove(nameToDelete);
                            Debug.LogFormat("Removing the value: {0}", nameToDelete);
                        }

                        _names.DeleteArrayElementAtIndex(indexToDelete);

                        // setting objectReferenceValue to null is required to remove the actual array element
                        _prefabs.GetArrayElementAtIndex(indexToDelete).objectReferenceValue = null;
                        _prefabs.DeleteArrayElementAtIndex(indexToDelete);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Empty");
                }
            }

            EditorGUILayout.PropertyField(_shroudPrefab);
            EditorGUILayout.PropertyField(_uiLockerPrefab);

            if (EditorGUI.EndChangeCheck() || isItemAdded) _hasChanges = true;

            PushGuiEnabledState(_hasChanges);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
                if (Validate())
                {
                    serializedObject.ApplyModifiedProperties();
                    UpdateNamesHashSet();
                    _hasChanges = false;
                }

            if (GUILayout.Button("Revert"))
            {
                serializedObject.Update();
                _hasChanges = false;
                _validationErrors.Clear();
            }

            EditorGUILayout.EndHorizontal();

            foreach (var errMsg in _validationErrors) DrawErrorLabel(errMsg);
            PopGuiEnabledState();
        }

        private bool Validate()
        {
            _validationErrors.Clear();
            var res = true;
            var firstOccurenceOfTheName = new Dictionary<ViewName, int>();
            for (var i = 0; i < _names.arraySize; i++)
            {
                var enumIndex = _names.GetArrayElementAtIndex(i).enumValueIndex;
                if (enumIndex < 0)
                {
                    res = false;
                    var err = string.Format("* Wrong enum index ({0}) at item #{1}", enumIndex, i);
                    _validationErrors.Add(err);
                    Debug.LogError(err);
                }
                else
                {
                    var viewName = _enumValues[enumIndex];
                    if (firstOccurenceOfTheName.TryGetValue(viewName, out var firstOccurence))
                    {
                        res = false;
                        var err = string.Format("* Duplicated name '{0}' at index {1}. First occurence index {2}",
                            viewName, i, firstOccurence);
                        _validationErrors.Add(err);
                        Debug.LogError(err);
                    }
                    else
                    {
                        firstOccurenceOfTheName.Add(viewName, i);
                    }
                }

                // checking prefabs
                if (_prefabs.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    var err = string.Format("* Prefab link is empty for item #{0}", i);
                    _validationErrors.Add(err);
                    Debug.LogError(err);
                }
            }

            return res;
        }

        private void UpdateNamesHashSet()
        {
            _addedDialogsNames.Clear();
            for (var i = 0; i < _names.arraySize; i++)
            {
                var enumValueIndex = _names.GetArrayElementAtIndex(i).enumValueIndex;
                if (enumValueIndex >= 0) _addedDialogsNames.Add(_enumValues[enumValueIndex]);
            }
        }

        private void PushGuiEnabledState(bool isEnabled)
        {
            _guiEnabledStateStack.Push(GUI.enabled);
            GUI.enabled = isEnabled;
        }

        private void PopGuiEnabledState()
        {
            if (_guiEnabledStateStack.Count > 0)
                GUI.enabled = _guiEnabledStateStack.Pop();
            else
                Debug.LogError("Can't pop GUI.enabled value from stack: the stack is empty");
        }

        private void DrawErrorLabel(string message)
        {
            var c = GUI.color;
            GUI.color = Color.red;
            var errorLabelStyle = new GUIStyle(GUI.skin.label);
            errorLabelStyle.normal.textColor = Color.red;
            EditorGUILayout.LabelField(message, errorLabelStyle);
            GUI.color = c;
        }

        #region Fields

        private SerializedProperty _names;
        private SerializedProperty _prefabs;
        private SerializedProperty _shroudPrefab;
        private SerializedProperty _uiLockerPrefab;

        #endregion

        #region Current editable fields values

        private ViewName _addedViewName;
        private Object _addedPrefab;

        #endregion

        #region Helper Fields

        private readonly HashSet<ViewName> _addedDialogsNames = new HashSet<ViewName>();
        private string[] _viewNames;
        private ViewName[] _enumValues;
        private bool _hasChanges;
        private readonly Stack<bool> _guiEnabledStateStack = new Stack<bool>();
        private readonly List<string> _validationErrors = new List<string>();

        private bool _addControlsExpanded;
        private bool _dialogsExpanded;

        #endregion
    }
}