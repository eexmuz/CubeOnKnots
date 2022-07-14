using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Instructions;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public abstract class Subsystem : MonoBehaviour, ISubsystem
    {
        #region Fields

        protected IList<IService> Services;

        /// <summary>
        ///     The notifications dispatcher.
        /// </summary>
        private INotificationDispatcher _notificationDispatcher;

        /// <summary>
        ///     Subsystem status
        /// </summary>
        private SubsystemStatus _status = SubsystemStatus.Offline;

        #endregion

        #region Public Properties

        public bool IsPaused { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual void Control()
        {
            SetStatus(SubsystemStatus.Controlling);
        }

        public virtual IEnumerable<IService> GetAllServices()
        {
            return Services;
        }

        public SubsystemStatus GetStatus()
        {
            return _status;
        }

        public virtual void Initialize(INotificationDispatcher notificationDispatcher)
        {
            _notificationDispatcher = notificationDispatcher;
            GetAllServices().ToList().ForEach(service => service.Initialize(notificationDispatcher));
            StartCoroutine(WaitForServicesStatus(ServiceStatus.Ready, SubsystemStatus.Ready));
        }

        public virtual void Pause(bool paused)
        {
            IsPaused = paused;
            GetAllServices().ToList().ForEach(service => service.Pause(paused));
        }

        public virtual void Run()
        {
            GetAllServices().ToList().ForEach(service => service.Run());
            StartCoroutine(WaitForServicesStatus(ServiceStatus.Running, SubsystemStatus.Running));
        }

        public virtual void StartSubsystem()
        {
            SetStatus(SubsystemStatus.Started);
        }

        #endregion

        #region Methods

        protected virtual void AdditionalStart()
        {
        }

        protected void Dispatch(NotificationType notificationType, NotificationParams notificationParams = null)
        {
            _notificationDispatcher.Dispatch(notificationType, notificationParams);
        }

        protected virtual void SetStatus(SubsystemStatus status)
        {
            _status = status;
        }

        /// <summary>
        ///     Waits for the status of the services and sets the subsystem's state after it
        /// </summary>
        protected IEnumerator WaitForServicesStatus(ServiceStatus status, SubsystemStatus subsystemStatusToSet)
        {
            yield return new VerifyServiceStatus(status, GetAllServices());
            SetStatus(subsystemStatusToSet);
        }

        [UsedImplicitly]
        private void Start()
        {
            SetStatus(SubsystemStatus.NotReady);
            AdditionalStart();
        }

        #endregion
    }
}