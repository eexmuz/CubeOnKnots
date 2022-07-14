using UnityEngine.SceneManagement;

public static class Scenes
{
    #region Constants

    public const int LEVEL_LOADER = 1;
    public const int GAME = 1;
    public const int PRELOADER = 0;

    #endregion
}

public static class PolyPlaySceneManager
{
    #region Public Methods and Operators

    public static int GetActiveSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    #endregion
}