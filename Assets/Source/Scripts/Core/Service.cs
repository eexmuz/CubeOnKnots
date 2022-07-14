using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using VertexDI.Login;

namespace Core
{
    public class Service : MonoBehaviour, IService
    {
        #region Fields

        /// <summary>
        ///     The notifications bus.
        /// </summary>
        private INotificationDispatcher _notificationsDispatcher;

        /// <summary>
        ///     Service status
        /// </summary>
        private ServiceStatus _status = ServiceStatus.Offline;

        #endregion

        #region Public Properties

        public bool IsPaused { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the observed notifications types. Service will automatically subscribe for these notifications
        /// </summary>
        protected virtual IList<NotificationType> ObservedNotifications => null;

        #endregion

        #region Public Methods and Operators

        public ServiceStatus GetStatus()
        {
            return _status;
        }

        public virtual void Initialize(INotificationDispatcher notificationDispatcher)
        {
            _notificationsDispatcher = notificationDispatcher;
            SetStatus(ServiceStatus.Ready);
        }

        public virtual void Pause(bool paused)
        {
            IsPaused = paused;
        }

        public virtual void Run()
        {
            SetStatus(ServiceStatus.Running);

            if (ObservedNotifications != null && _notificationsDispatcher != null)
                foreach (var notificationType in ObservedNotifications)
                    _notificationsDispatcher.Subscribe(notificationType, OnNotification);
        }

        #endregion

        #region Methods

        /*
        protected IEnumerator waitForRunningAndLoggedIn(ILoginService loginService, OperationResult waitingResult, uint loginWaitingTimeout_ms = 5000) {

            if (getStatus() != ServiceStatus.Running) {
                yield return waitForStatus (ServiceStatus.Running);
            }

            if (!loginService.IsLoggedIn) {
                OperationResult loginWaitingResult = new OperationResult();
                yield return new WaitForLogin (loginService, loginWaitingResult, loginWaitingTimeout_ms);
                if (!loginWaitingResult.Success) {
                    waitingResult.Error = loginWaitingResult.Error;
                }
            }
        }
        */

        protected void Dispatch(NotificationType notificationType, NotificationParams notificationParams = null)
        {
            _notificationsDispatcher?.Dispatch(notificationType, notificationParams);
        }

        protected void Log(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Debug.LogFormat("[{0}] {1}", GetType().FullName, message);
        }

        protected void LogError(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Debug.LogErrorFormat("[{0}] {1}", GetType().FullName, message);
        }

        protected void LogWarning(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Debug.LogWarningFormat("[{0}] {1}", GetType().FullName, message);
        }

        protected virtual void OnNotification(NotificationType notificationType, NotificationParams notificationParams)
        {
        }

        protected virtual void SetStatus(ServiceStatus status)
        {
            _status = status;
        }

        protected void Subscribe(NotificationType notificationType, NotificationHandler notificationHandler)
        {
            _notificationsDispatcher?.Subscribe(notificationType, notificationHandler);
        }

        protected IEnumerator WaitForStatus(ServiceStatus status)
        {
            while (_status != status)
            {
                Debug.Log("Waiting");
                yield return 0;
            }
        }

        #endregion
    }
}