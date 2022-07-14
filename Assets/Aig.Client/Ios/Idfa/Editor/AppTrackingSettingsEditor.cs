using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Aig.Client.Ios.Idfa.Editor
{
    [CustomEditor(typeof(AppTrackingSettings))]
    public class AppTrackingSettingsEditor : UnityEditor.Editor 
    {
        private AppTrackingSettings _settings;

        private SerializedProperty googleDocId;
        private SerializedProperty localizedDescriptionList;

        private WWW _www;
        private List<string> _csvRows = new List<string>();

        private void OnEnable()
        {
            _settings = (AppTrackingSettings) target;

            googleDocId = serializedObject.FindProperty("googleDocId");
            localizedDescriptionList = serializedObject.FindProperty("localizedDescriptionList");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Localized Description List", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(googleDocId);

            GUI.enabled = googleDocId.stringValue.Length > 0;
            if (GUILayout.Button("Load"))
            {
                UpdateLocalizedDescriptionList();
            }
            GUI.enabled = true;

            if (localizedDescriptionList.arraySize == 0)
            {
                EditorGUILayout.LabelField("Localized Description List not loaded!", EditorStyles.boldLabel);
                return;
            }

            for (var i = 0; i < localizedDescriptionList.arraySize; i++)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(localizedDescriptionList.GetArrayElementAtIndex(i));
                GUILayout.Space(-20);
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_settings);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void UpdateLocalizedDescriptionList()
        {
            _www = new WWW($"https://docs.google.com/spreadsheets/d/{googleDocId.stringValue}/gviz/tq?tqx=out:csv&sheet=texts");

            EditorApplication.update += Tick;
        }

        private void Tick()
        {
            if (_www.isDone)
            {
                if (!string.IsNullOrEmpty(_www.error))
                {
                    Debug.LogError("WWW failed: " + _www.error);
                }
                else
                {
                    ParseLocalizationCsv(_www.text);
                }

                EditorApplication.update -= Tick;

                _www.Dispose();
            }
        }

        private void ParseLocalizationCsv(string text)
        {
            var csvData = text;

            _settings.localizedDescriptionList.Clear();

            // convert text into rows by splitting along line breaks
            _csvRows = csvData.Split("\n"[0]).ToList();

            for (var i = 0; i < _csvRows.Count; i++)
            {
                var row = _csvRows[i];
                var substringRow = row.Substring(1, row.Length - 2);
                var fields = substringRow.Split(new[] {"\",\""}, StringSplitOptions.None);

                for (var j = 0; j < fields.Length; j++)
                {
                    fields[j] = fields[j].Replace("\"\"", "\"");
                }

                _settings.localizedDescriptionList.Add(new LocalizedDescription()
                {
                    langCode = fields[0],
                    langName = fields[1],
                    description = fields[2]
                });
            }

            serializedObject.UpdateIfRequiredOrScript();
            EditorUtility.SetDirty(_settings);
            serializedObject.ApplyModifiedProperties();
        }
    }
}