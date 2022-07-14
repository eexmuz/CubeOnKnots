using System;
using Assets.SimpleAndroidNotifications;
using Core.Settings;
using Core.Attributes;
using UnityEngine;

namespace Core.Services
{
    [InjectionAlias(typeof(ILocalNotificationService))]
    public class LocalNotificationService : Service, ILocalNotificationService
    {
        [Inject]
        private LocalNotificationSettings _localNotificationSettings;

        [Inject]
        private ILocaleService _localeService;

        public void AddNotification(LocalNotificationType notificationType, TimeSpan timeSpan, string soundName = "")
        {
            RemoveNotification(notificationType);

            var notificationId = (int)notificationType;

            LocalNotificationInfo localNotification = GetNotificationIdByType(notificationType);

#if UNITY_ANDROID && !UNITY_EDITOR
            NotificationManager.Send(notificationId, timeSpan, _localeService.GetString(localNotification.title), _localeService.GetString(localNotification.message), localNotification.color);
#elif UNITY_IOS && !UNITY_EDITOR
            LocalNotificationSender.SendNotification(notificationId, timeSpan, _localeService.GetString(localNotification.title), _localeService.GetString(localNotification.message), localNotification.color);
#endif
        }

        public void RemoveNotification(LocalNotificationType notificationType)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            NotificationManager.Cancel((int)notificationType);
#elif UNITY_IOS && !UNITY_EDITOR
            LocalNotificationSender.CancelNotification((int)notificationType);
#endif
        }

        public void CancelAllLocalNotification()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            NotificationManager.CancelAll();
#elif UNITY_IOS && !UNITY_EDITOR
            LocalNotificationSender.ClearNotifications();
#endif
        }

        private LocalNotificationInfo GetNotificationIdByType(LocalNotificationType notificationType)
        {
            foreach (LocalNotificationInfo localNotification in _localNotificationSettings.localNotifications)
            {
                if (localNotification.type == notificationType)
                {
                    return localNotification;
                }
            }

            return null;
        }
    }
}
