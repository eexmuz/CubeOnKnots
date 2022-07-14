using System;
using Core;

namespace Core.Services
{
    public interface ILocalNotificationService : IService
    {
        void AddNotification(LocalNotificationType notificationType, TimeSpan timeSpan, string soundName = "");

        void RemoveNotification(LocalNotificationType notificationType);

        void CancelAllLocalNotification();
    }
}
