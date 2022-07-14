namespace Core
{
    /// <summary>
    ///     Status used for services
    /// </summary>
    public enum ServiceStatus
    {
        Offline,
        Ready,
        Running
    }

    /// <summary>
    ///     Interface for an application service. An application service is hosted by a subsystem
    ///     and performs the major pieces of work for a subsystem.
    /// </summary>
    public interface IService
    {
        #region Public Properties

        /// <summary>
        ///     Get the pause status
        /// </summary>
        bool IsPaused { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the status of the service
        /// </summary>
        ServiceStatus GetStatus();

        /// <summary>
        ///     Handle any setup and initialization.
        ///     Phase 1 of service startup sequencing.
        /// </summary>
        void Initialize(INotificationDispatcher notificationDispatcher);

        /// <summary>
        ///     Called when the App is put into a suspended state
        /// </summary>
        void Pause(bool paused);

        /// <summary>
        ///     Put into the running mode.
        ///     Phase 2 of system startup sequencing.
        /// </summary>
        void Run();

        #endregion
    }
}