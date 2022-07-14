using System.Collections.Generic;

namespace Core
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        #region Fields

        private readonly Dictionary<NotificationType, HashSet<NotificationHandler>> _subscriptions =
            new Dictionary<NotificationType, HashSet<NotificationHandler>>();

        #endregion

        #region Public Methods and Operators

        public void Dispatch(NotificationType notificationType, NotificationParams notificationParams = null)
        {
            if (_subscriptions.TryGetValue(notificationType, out var currentSubscribers))
            {
                var subscribersCopy = new HashSet<NotificationHandler>(currentSubscribers);
                foreach (var handler in subscribersCopy) handler(notificationType, notificationParams);
            }
        }

        public void Subscribe(NotificationType notificationType, NotificationHandler handler)
        {
            if (!_subscriptions.TryGetValue(notificationType, out var currentSubscribers))
            {
                currentSubscribers = new HashSet<NotificationHandler>();
                _subscriptions[notificationType] = currentSubscribers;
            }

            currentSubscribers.Add(handler);
        }

        public void Unsubscribe(NotificationType notificationType, NotificationHandler handler)
        {
            if (_subscriptions.TryGetValue(notificationType, out var currentSubscribers))
                currentSubscribers.Remove(handler);
        }

        #endregion
    }
}