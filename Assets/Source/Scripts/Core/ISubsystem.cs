using System.Collections.Generic;

namespace Core
{
    /// <summary>
    ///     Status used for subsystems
    /// </summary>
    public enum SubsystemStatus
    {
        Offline,
        NotReady,
        Ready,
        Started,
        Running,

        Controlling
        //Paused
    }

    /// <summary>
    ///     Interface for an application subsystem. A major piece of functionality in the application.
    /// </summary>
    public interface ISubsystem
    {
        #region Public Properties

        /// <summary>
        ///     Get the pause status
        /// </summary>
        bool IsPaused { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Called to tell the subsystem to take control of the current operations of the application.
        /// </summary>
        void Control();

        /// <summary>
        ///     Get all the objects that a subsystem considers a service
        /// </summary>
        IEnumerable<IService> GetAllServices();

        /// <summary>
        ///     Get the status of the subsystem
        /// </summary>
        SubsystemStatus GetStatus();

        /// <summary>
        ///     Handle any setup and initialization.
        ///     Phase 1 of system startup sequencing.
        ///     Pass INotificationDispatcher for app-wide notifications handling and dispatching support
        /// </summary>
        void Initialize(INotificationDispatcher notificationsDispatcher);

        /// <summary>
        ///     Called when the App is put into a suspended state
        /// </summary>
        void Pause(bool paused);

        /// <summary>
        ///     Put into the running mode. At this point you can interact with any other subsytem.
        ///     Phase 3 of system startup sequencing.
        /// </summary>
        void Run();

        /// <summary>
        ///     Start any and all operations.
        ///     Phase 2 of system startup sequencing.
        /// </summary>
        void StartSubsystem();

        #endregion
    }
}