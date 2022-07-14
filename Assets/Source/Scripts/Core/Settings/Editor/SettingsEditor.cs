using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Core.Settings.Editor
{
    /// <summary>
    ///     Custom editor to edit a Settings object in the scene
    /// </summary>
    [CustomEditor(typeof(SettingsContainer))]
    public class SettingsEditor : UnityEditor.Editor
    {
        private ReorderableList _settingsList;

        private void OnEnable()
        {
            var allSettings = serializedObject.FindProperty("allSettings");
            _settingsList = new ReorderableList(serializedObject, allSettings, true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "All Settings"); },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _settingsList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element,
                        GUIContent.none);
                },

                onAddDropdownCallback = (buttonRect, l) =>
                {
                    var menu = new GenericMenu();
                    GetSubclassesList(typeof(ISettings)).ToList().ForEach(type =>
                    {
                        menu.AddItem(new GUIContent(type.Name), false, OnCreateHandler,
                            new SettingsCreateParams {ObjectType = type});
                    });
                    menu.ShowAsContext();
                }
            };
        }

        private void OnCreateHandler(object targetObject)
        {
            var data = (SettingsCreateParams) targetObject;
            var index = _settingsList.serializedProperty.arraySize;
            _settingsList.serializedProperty.arraySize++;
            _settingsList.index = index;
            var element = _settingsList.serializedProperty.GetArrayElementAtIndex(index);

            element.objectReferenceValue = CreateAsset(data.ObjectType.Name);

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        ///     Called to create custom GUI controls
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            _settingsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        ///     Retrieves the list of classes that are a subclass or implement the class.
        /// </summary>
        private static IEnumerable<Type> GetSubclassesList(Type classType)
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where classType.IsAssignableFrom(type) && type.IsClass
                select type;
        }

        /// <summary>
        ///     create a new instance of the settings and save it to disk
        /// </summary>
        private static ScriptableObject CreateAsset(string name)
        {
            var asset = CreateInstance(GetType(name));

            if (asset == null)
            {
                return null;
            }

            var path = EditorUtility.SaveFilePanelInProject("Save Settings Asset", name, "asset",
                "Save Settings Asset");

            if (path.Length == 0) return null;

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }

        /// <summary>
        ///     Get the type of a settings object
        /// </summary>
        private static Type GetType(string name)
        {
            switch (name)
            {
                case "ApplicationSettings":
                    return typeof(ApplicationSettings);
                case "GameSettings":
                    return typeof(GameSettings);
                case "UiSystemSettings":
                    return typeof(UiSystemSettingsAsset);
                case "GameAudioSettings":
                    return typeof(GameAudioSettings);
                case "IapProductsSettings":
                    return typeof(IapProductsSettings);
                case "LocalNotificationSettings":
                    return typeof(LocalNotificationSettings);
                default:
                    return typeof(void);
            }
        }

        private struct SettingsCreateParams
        {
            public Type ObjectType;
        }
    }
}