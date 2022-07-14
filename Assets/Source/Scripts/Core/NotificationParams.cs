namespace Core
{
    public class NotificationParams
    {
        #region Public Properties

        public object Data { get; set; }

        #endregion

        #region Public Methods and Operators

        public static NotificationParams Get(object data = null)
        {
            var res = new NotificationParams
            {
                Data = data
            };
            return res;
        }

        #endregion
    }
}