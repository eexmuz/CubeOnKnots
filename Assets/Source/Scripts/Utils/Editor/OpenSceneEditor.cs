using UnityEditor;
using UnityEditor.SceneManagement;

public static class OpenSceneEditor
{
    private const string MenuPrefix = "Utils/Open Scene/";

    [MenuItem(MenuPrefix + "Preloader", false, 0)]
    private static void OpenPreloader()
    {
        OpenCommonScene("Preloader");
    }

    [MenuItem(MenuPrefix + "ChainBlock", false, 1)]
    private static void OpenLobby()
    {
        OpenCommonScene("ChainBlockArt");
    }

    [MenuItem(MenuPrefix + "UI", false, 2)]
    private static void OpenUI()
    {
        OpenCommonScene("Hidden/UI");
    }

    private static void OpenCommonScene(string name)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene($"Assets/Source/Content/Scenes/{name}.unity");
        }
    }

    private static void OpenLevelScene(string name)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(string.Format("Assets/Source/Content/Scenes/Levels/{0}/{0}.unity", name));
        }
    }
}