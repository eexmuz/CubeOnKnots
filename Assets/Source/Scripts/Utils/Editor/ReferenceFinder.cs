using System;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Object = UnityEngine.Object;

public class ReferenceFinder : EditorWindow
{
    private List<string> FoundObjects = new List<string>();
    private List<int> FoundObjectsCount = new List<int>();
    [SerializeField]
    private List<Object> FoundGameObjects;
    [SerializeField]
    private List<int> FoundGameObjectsCount;
    
    private int CurrentProgress;
    private int FilesCount;
    private string FileName;
    private bool ShowCount = true;

    private Dictionary<string, bool> AvailableTypes = new Dictionary<string, bool>()
    {
        {"*.prefab", true},
        {"*.mat", true},
        {"*.unity", true},
        {"*.asset", true},
        {"*.controller", true},
        {"*.anim", true}
    };

    private string Guid;

    private void SetUpSearch(string guid)
    {
        Guid = guid;
    }

    private void StartSearch(string guid)
    {
        if (string.IsNullOrEmpty(guid))
            return;

        Guid = guid;
        CurrentProgress = 0;
        FoundObjects = new List<string>();
        FoundObjectsCount = new List<int>();
        FoundGameObjects = new List<Object>();
        FoundGameObjectsCount = new List<int>();
        List<string> files = new List<string>();

        foreach (KeyValuePair<string, bool> availableType in AvailableTypes)
        {
            if (availableType.Value)
                files.AddRange(Directory.GetFiles("Assets/", availableType.Key, SearchOption.AllDirectories).ToList());
        }

        FilesCount = files.Count;
        List<List<string>> devidedPaths = Split(files, SystemInfo.processorCount - 1);

        foreach (List<string> paths in devidedPaths)
        {
            List<string> paths1 = paths;
            Thread thread = new Thread(() => FindReferences(paths1));
            thread.Start();
        }
    }

    public static List<List<T>> Split<T>(List<T> collection, int chunkCount)
    {
        int size = collection.Count() / chunkCount;
        List<List<T>> chunks = new List<List<T>>();
        for (var i = 0; i < chunkCount; i++)
            chunks.Add(collection.Skip(i * size).Take(i < chunkCount - 1 ? size : collection.Count - i * size).ToList());

        return chunks;
    }

    private void FindReferences(List<string> pahtList)
    {
        Thread thread = Thread.CurrentThread;
        foreach (string file in pahtList)
        {
            CurrentProgress++;
            FileName = file;
            using (StreamReader streamReader = new StreamReader(file))
            {
                var stream = streamReader.ReadToEnd();

                var count = CountInString(stream, Guid, ShowCount);
                if (count > 0)
                {
                    lock (FoundObjects)
                    {
                        FoundObjects.Add(file);
                    }
                    lock (FoundObjectsCount)
                    {
                        FoundObjectsCount.Add(count);
                    }
                }
            }
            GC.Collect();
            Thread.Sleep(5);
        }
        thread.Join();
    }
    
    private int CountInString(string str, string value, bool all)
    {
        var count = 0;
        if (String.IsNullOrEmpty(value))
        {
            return count;
        }

        List<int> indexes = new List<int>();
        for (int index = 0;; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
            {
                return count;
            }

            count++;

            if (all == false)
            {
                return count;
            }
        }
    }

    private Vector2 ScrollPosition;

    void Update()
    {
        if(CurrentProgress%2 == 0)
            Repaint();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, 50));
        GUILayout.BeginHorizontal();

        //GUILayout.BeginVertical();
        ShowCount = GUILayout.Toggle(ShowCount, "Show Count");
        GUILayout.TextArea(Guid);
        //GUILayout.EndVertical();

        AvailableTypes["*.prefab"] = GUILayout.Toggle(AvailableTypes["*.prefab"], "Prefabs");
        AvailableTypes["*.mat"] = GUILayout.Toggle(AvailableTypes["*.mat"], "Materials");
        AvailableTypes["*.unity"] = GUILayout.Toggle(AvailableTypes["*.unity"], "Scenes");
        AvailableTypes["*.asset"] = GUILayout.Toggle(AvailableTypes["*.asset"], "Assets");
        AvailableTypes["*.anim"] = GUILayout.Toggle(AvailableTypes["*.anim"], "Anims");
        AvailableTypes["*.controller"] = GUILayout.Toggle(AvailableTypes["*.controller"], "Controllers");

        if (GUILayout.Button("Search", GUILayout.MinWidth(150), GUILayout.MinHeight(50)))
            StartSearch(Guid);//ReferenceIndexer.Instance.IndexAllFiles();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.Space(20);

        //float progress = FilesCount == 0 ? 0f : CurrentProgress * 100f / FilesCount;
        //GUILayout.Label("Progress: " + progress + "%");
        GUILayout.Label("Progress: " + CurrentProgress + " / " + FilesCount);
        GUILayout.Label("File: " + FileName);

        GUILayout.Space(20);

        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, true, true, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 50));
        lock (FoundObjects)
        {
            if (FoundObjects.Count > 0 && FoundGameObjects != null && FoundObjects.Count - FoundGameObjects.Count > 0)
            {
                for (int i = FoundGameObjects.Count; i < FoundObjects.Count; i++)
                {
                    Object newObject = AssetDatabase.LoadAssetAtPath<Object>(FoundObjects[i]);
                    if (FoundGameObjects.Contains(newObject))
                    {
                        continue;
                    }

                    lock (FoundObjectsCount)
                    {
                        FoundGameObjectsCount.Add(FoundObjectsCount[i]);
                    }
                    FoundGameObjects.Add(newObject);
                }
            }
        }
        if (FoundGameObjects != null)
        {
            for (var i = 0; i < FoundGameObjects.Count; i++)
            {
                var foundGameObject = FoundGameObjects[i];
                if (foundGameObject == null)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(foundGameObject.name, foundGameObject, foundGameObject.GetType());
                GUILayout.Label(FoundGameObjectsCount[i].ToString());

                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
    }

    [MenuItem("Assets/Find All References", false, 25)]
    public static void ShowSearch()
    {
        Object mainObject = Selection.objects.First();

        string path = AssetDatabase.GetAssetPath(mainObject);
        string guid = AssetDatabase.AssetPathToGUID(path);

        ReferenceFinder popup = GetWindow<ReferenceFinder>(true, "Reference Finder", true);
        popup.SetUpSearch(guid);
    }
}