using System.Collections.Generic;
using Core.Services;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (LocalizedItem), true)]
public class LocalizedItemEditor : Editor
{
    private static Dictionary<string, LocaleData> _locales;
    private TextMeshProUGUI _itemLabel;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        /*
        var objectEditor = (LocalizedItem) target;

        if (_itemLabel == null)
        {
            _itemLabel = objectEditor.GetComponent<TextMeshProUGUI>();
        }

        if (_itemLabel != null)
        {
            if (_locales == null)
            {
                if (GUILayout.Button("Load Localization file"))
                {
                    LoadLocalizationDataFromFile();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(objectEditor.StringId) == false)
                {
                    if (_locales.ContainsKey(objectEditor.StringId) == false)
                    {
                        if (GUILayout.Button("Add to source"))
                        {
                            var text = _itemLabel.text;
                            if (string.IsNullOrEmpty(text) == false)
                            {
                                SetLocale(new LocaleData(objectEditor.StringId, text));
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Change in source"))
                        {
                            var text = _itemLabel.text;
                            if (string.IsNullOrEmpty(text) == false)
                            {
                                SetLocale(new LocaleData(objectEditor.StringId, text));
                            }
                        }
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(objectEditor);
            }
        }
        */
    }
    /*
    [MenuItem("Utils/Localization/Load Localization file")]
    public static void LoadLocalizationDataFromFile()
    {
        var txt = Resources.Load<TextAsset>("localization/text_en");
        if (txt != null)
        {
            var localeData = JSON.Parse(txt.text);

            for (var i = 0; i < localeData.Count; i++)
            {
                if (localeData[i]["text"] == null)
                {
                    localeData[i]["text"] = " ";
                }

                SetLocale(new LocaleData(localeData[i]["id"], localeData[i]["text"]));
            }
        }
        else
        {
            Debug.Log("Localization: File Resources/localization/text_en.txt not found");
        }
    }

    [MenuItem("Utils/Localization/Save Localization file")]
    public static void SaveLocalizationDataFile()
    {
        if ((_locales != null) && (_locales.Count > 0))
        {
            var arr = new JSONArray();
            foreach (var locData in _locales)
            {
                var item = new JSONClass();
                item["id"] = locData.Value.LocaleId;
                item["text"] = locData.Value.Text;
                arr.Add(item);
            }

            JSONUtils.SaveToFile(arr, "Assets/Resources/localization/text_en.txt");
        }
        else
        {
            Debug.Log("Load Localization file before saving!");
        }
    }

    private static void SetLocale(LocaleData localeData)
    {
        if (_locales == null)
        {
            _locales = new Dictionary<string, LocaleData>();
        }

        if (_locales.ContainsKey(localeData.LocaleId))
        {
            _locales[localeData.LocaleId] = localeData;
        }
        else
        {
            _locales.Add(localeData.LocaleId, localeData);
        }
    }

    public static void AddLocale(LocaleData localeData)
    {
        if (_locales == null)
        {
            Debug.Log("Load Localization file first to add locale!");
            return;
        }

        if (_locales.ContainsKey(localeData.LocaleId) == false)
        {
            _locales.Add(localeData.LocaleId, localeData);
            Debug.LogWarningFormat("Add locale stringId: {0} text: {1}", localeData.LocaleId, localeData.Text);
        }
    }

    [MenuItem("Utils/Localization/Localize All Scene Labels")]
    public static void LocalizeAllSceneLabels()
    {
        if (_locales == null)
        {
            LoadLocalizationDataFromFile();
        }

        var objects = ObjectsEditorsUtils.GetAllSceneObjects();
        for (var i = 0; i < objects.Length; i++)
        {
            var locItem = objects[i].GetComponent<LocalizedItem>();
            if ((locItem != null) && (string.IsNullOrEmpty(locItem.StringId) == false))
            {
                var label = objects[i].GetComponent<TextMeshPro>();
                if (label != null)
                {
                    label.text = ((_locales != null) && _locales.ContainsKey(locItem.StringId))
                        ? _locales[locItem.StringId].Text
                        : locItem.StringId;
                }
            }
        }
    }
    */
}