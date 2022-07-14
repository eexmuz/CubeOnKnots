using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

using UnityEngine;
using Utility;

public class LocalizationMenu : EditorWindow
{
    private bool _isLoading = false;
    private bool _isLoaded = false;
    private bool _isParsed = false;
    private bool _isError = false;
    private List<string> _csvRows = new List<string>();
    private List<string> _langs = new List<string>();

    [MenuItem("Utils/Localization/Load Localization")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LocalizationMenu));
    }

    private void OnGUI()
    {
        GUILayout.Label("Load from google docs", EditorStyles.boldLabel);

        GUI.enabled = !_isLoading;

        if (GUILayout.Button("Load"))
        {
            var www = new WWW("https://docs.google.com/spreadsheets/d/1rHn1NkH1cTrij9pZYY1cjr1LFmib_F2o8TTqeyQokGg/gviz/tq?tqx=out:csv&sheet=texts");

            _isLoading = true;
            _isLoaded = false;
            _isError = false;
            _isParsed = false;

            ContinuationManager.Add(() => www.isDone, () =>
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.Log("WWW failed: " + www.error);
                    _isError = true;
                    _isLoading = false;
                }
                else
                {
                    Debug.Log("WWW result : " + www.text);
                    _isLoading = false;
                    _isLoaded = true;
                    ParseLocalizationCSV(www.text);
                }
            });
        }

        GUI.enabled = true;

        if (_isLoaded)
        {
            GUILayout.Label("Loaded!", EditorStyles.boldLabel);
        }

        if(_isParsed)
        {
            GUILayout.Label("Created text files for languages:", EditorStyles.boldLabel);

            for (var i = 1; i < _langs.Count; i++)
            {
                GUILayout.Label(_langs[i], EditorStyles.boldLabel);
            }
        }

        if (_isLoading)
        {
            GUILayout.Label("Loading...", EditorStyles.boldLabel);
        }

        if (_isError)
        {
            GUILayout.Label("Error!!!", EditorStyles.boldLabel);
        }
    }

    private void ParseLocalizationCSV(string text)
    {
        string csvData = text;

        // convert text into rows by splitting along line breaks
        _csvRows = csvData.Split("\n"[0]).ToList();

        string headerRow = _csvRows[0];
        headerRow = headerRow.Substring(1, headerRow.Length-2);
         _langs = headerRow.Split(new string[] { "\",\"" }, StringSplitOptions.None).ToList();

        _csvRows.RemoveAt(0);

        var dictionaries = new Dictionary<string, Dictionary<string, string>>();

        for (var i = 1; i < _langs.Count; i++)
        {
            if(string.IsNullOrEmpty(_langs[i]))
                continue;

            dictionaries.Add(_langs[i], new Dictionary<string, string>());
            Dictionary<string, string> dictionary = dictionaries[_langs[i]];

            foreach (string row in _csvRows)
            {
                var substringRow = row.Substring(1, row.Length - 2);
                List<string> fields = substringRow.Split(new[] { "\",\"" }, StringSplitOptions.None).ToList();

                if (dictionary.ContainsKey(fields[0]) == false)
                {
                    string field = fields[i];
                    field = field.Replace("\"\"", "\"");
                    dictionary.Add(fields[0], field);
                }
            }
        }

        foreach (KeyValuePair<string, Dictionary<string, string>> dictionary in dictionaries)
        {
            var dirPath = "Assets/Source/Content/Resources/Localization/";
            Directory.CreateDirectory(dirPath);

            string path = dirPath + "text_" + dictionary.Key + ".txt";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            //Write some text to the text_.txt file
            StreamWriter writer = new StreamWriter(path, true, new UTF8Encoding(false));

            writer.WriteLine("[");

            foreach (KeyValuePair<string, string> textPair in dictionary.Value)
            {
                writer.WriteLine("    {");
                writer.WriteLine("        \"id\": \"" + textPair.Key + "\",");
                writer.WriteLine("        \"text\": \"" + textPair.Value + "\"");
                writer.WriteLine("    },");
            }

            writer.WriteLine("]");

            writer.Close();

            //Re-import the file to update the reference in the editor
            AssetDatabase.ImportAsset(path);
            TextAsset asset = Resources.Load("text_" + dictionary.Key) as TextAsset;
        }

        _isParsed = true;
    }
}
