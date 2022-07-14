namespace Core.Services
{
    public interface IGameOptionsService : IService
    {
        #region Public Properties

        bool Sound { get; set; }
        bool Music { get; set; }
        bool Vibration { get; set; }

        #endregion

        #region Public Methods and Operators

        #endregion
    }
}