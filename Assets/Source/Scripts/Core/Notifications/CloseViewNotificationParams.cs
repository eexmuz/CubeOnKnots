namespace Core.Notifications
{
    public class CloseViewNotificationParams : NotificationParams
    {
        #region Fields

        public bool Instant;
        public ViewName ViewName;

        #endregion

        #region Public Methods and Operators

        public static CloseViewNotificationParams Get(ViewName viewName, bool instant = false)
        {
            var res = new CloseViewNotificationParams
            {
                ViewName = viewName,
                Instant = instant
            };
            return res;
        }

        #endregion
    }
}