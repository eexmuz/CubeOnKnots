using System.Collections;
using Core.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services
{
    [InjectionAlias(typeof(ILevelLoaderService))]
    public class LevelLoaderService : Service, ILevelLoaderService
    {
        #region Public Properties

        public int LastSavedLevel { get; set; } = Scenes.GAME;
        public int LevelForLoad { get; set; } = Scenes.GAME;

        #endregion

        #region Public Methods and Operators

        public void LoadLevel(int level, bool forceLoad = false)
        {
            if (LevelForLoad == level && !forceLoad) return;

            StartCoroutine(LoadLevelAsync(level));
        }

        public void LogLevel()
        {
        }

        #endregion

        #region Methods

        private IEnumerator LoadLevelAsync(int level)
        {
            LevelForLoad = level;

            yield return Resources.UnloadUnusedAssets();

            yield return SceneManager.LoadSceneAsync(Scenes.LEVEL_LOADER);

            yield return Resources.UnloadUnusedAssets();
        }

        #endregion
    }
}