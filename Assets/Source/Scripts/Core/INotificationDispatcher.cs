namespace Core
{
    public delegate void NotificationHandler(NotificationType notificationType,
        NotificationParams notificationParams = null);

    public interface INotificationDispatcher
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Dispatch notification
        /// </summary>
        void Dispatch(NotificationType notificationType, NotificationParams notificationParams = null);

        /// <summary>
        ///     Subscribe for the specified notifications type
        /// </summary>
        void Subscribe(NotificationType notificationType, NotificationHandler handler);

        /// <summary>
        ///     Unsubscribe the specified notification type
        /// </summary>
        void Unsubscribe(NotificationType notificationType, NotificationHandler handler);

        #endregion
    }
}