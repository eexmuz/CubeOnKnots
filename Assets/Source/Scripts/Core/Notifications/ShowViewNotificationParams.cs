using System;
using Core.Services;

namespace Core.Notifications
{
    public class ShowViewNotificationParams : NotificationParams
    {
        #region Public Properties

        public object Data { get; set; }

        public ViewCreationOptions Options { get; private set; }
        public ViewName ViewName { get; private set; }

        #endregion

        #region Public Methods and Operators

        public static ShowViewNotificationParams Get(ViewName viewName,
            ViewCreationOptions options = ViewCreationOptions.None, object data = null)
        {
            var res = new ShowViewNotificationParams
            {
                ViewName = viewName,
                Options = options,
                Data = data
            };
            return res;
        }

        #endregion
    }
}