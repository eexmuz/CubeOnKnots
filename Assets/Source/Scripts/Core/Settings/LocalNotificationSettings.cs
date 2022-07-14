using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Settings
{
    [Serializable]
    public class LocalNotificationInfo
    {
        public LocalNotificationType type;
        public string title;
        public string message;
        public Color color;
    }
    
    [CreateAssetMenu(menuName = "Settings/Local Notification Settings")]
    public class LocalNotificationSettings : ScriptableObject, ISettings
    {
        #region Fields

        public List<LocalNotificationInfo> localNotifications;

        #endregion
    }
}