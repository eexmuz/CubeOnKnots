using Core.Attributes;

namespace Core.Services
{
    [InjectionAlias(typeof(IGameService))]
    public class GameService : Service, IGameService
    {
        public LevelConfig CurrentLevel { get; private set; }

        public override void Run()
        {
            Subscribe(NotificationType.LevelLoaded, OnLevelLoaded);
            base.Run();
        }

        private void OnLevelLoaded(NotificationType notificationType, NotificationParams notificationParams)
        {
            CurrentLevel = (LevelConfig) notificationParams.Data;
        }
    }
}