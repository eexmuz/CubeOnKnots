namespace Core.Services
{
    public interface ILevelLoaderService : IService
    {
        #region Public Properties

        int LastSavedLevel { get; set; }
        int LevelForLoad { get; set; }

        #endregion

        #region Public Methods and Operators

        void LoadLevel(int level, bool forceLoad = false);

        void LogLevel();

        #endregion
    }
}