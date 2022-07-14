using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    ///     The MonoBehaviour which automates dependencies injection into itself
    /// </summary>
    public class DIBehaviour : MonoBehaviour
    {
        #region Fields

        private bool _started;
        private IDictionary<NotificationType, HashSet<NotificationHandler>> _subscriptions;

        #endregion

        #region Public Methods and Operators

        public void Dispatch(NotificationType notificationType, NotificationParams notificationParams = null)
        {
            App.SharedNotificationDispatcher.Dispatch(notificationType, notificationParams);
        }

        public void Subscribe(NotificationType notificationType, NotificationHandler handler)
        {
            if (_subscriptions == null)
                _subscriptions = new Dictionary<NotificationType, HashSet<NotificationHandler>>();

            HashSet<NotificationHandler> currentNotificationHandlers;

            if (!_subscriptions.TryGetValue(notificationType, out currentNotificationHandlers))
            {
                currentNotificationHandlers = new HashSet<NotificationHandler>();
                _subscriptions[notificationType] = currentNotificationHandlers;
            }

            if (currentNotificationHandlers.Add(handler))
                App.SharedNotificationDispatcher.Subscribe(notificationType, handler);
        }

        public void Unsubscribe(NotificationType notificationType, NotificationHandler handler)
        {
            if (_subscriptions == null) return;

            if (!_subscriptions.TryGetValue(notificationType, out var currentNotificationHandlers)) return;

            if (currentNotificationHandlers.Remove(handler))
                App.SharedNotificationDispatcher.Unsubscribe(notificationType, handler);
        }

        #endregion

        #region Methods

        protected virtual void Awake()
        {
            App.Inject(this);
            OnAppInitialized();
            _started = true;
        }

        /// <summary>
        ///     Override this method to start working with injected values.
        /// </summary>
        protected virtual void OnAppInitialized()
        {
        }

        protected virtual void OnPauseStateChanged(bool state)
        {
        }

        protected virtual void OnDestroy()
        {
            if (_subscriptions != null)
                foreach (var kvp in _subscriptions)
                    foreach (var handler in kvp.Value)
                        App.SharedNotificationDispatcher.Unsubscribe(kvp.Key, handler);

            _subscriptions = null;
        }

        protected virtual IEnumerator WaitForStarted()
        {
            while (!_started) yield return 0;
        }

        protected void ExecuteOnStarted(System.Action action)
        {
            StartCoroutine(ExecuteOnStarted_co(action));
        }

        private IEnumerator ExecuteOnStarted_co(System.Action action)
        {
            yield return WaitForStarted();
            action?.Invoke();
        }
        
        #endregion
    }
}