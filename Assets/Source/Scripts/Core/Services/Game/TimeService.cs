using System.Collections;
using Core.Attributes;
using UnityEngine;

namespace Core.Services
{
    [InjectionAlias(typeof(ITimeService))]
    public class TimeService : Service, ITimeService
    {
        [Inject]
        private IPlayerDataService _playerDataService;
        
        private Coroutine _levelTimerCoroutine;
        
        public override void Run()
        {
            Subscribe(NotificationType.DisplayingDialogStateChanged, DisplayingDialogStateChangedHandler);
            InitializeTimers();

            base.Run();
        }

        private void InitializeTimers()
        {
            StartCoroutine(SessionTimer());
            _levelTimerCoroutine = StartCoroutine(GameplayTimer());
        }
        
        private void DisplayingDialogStateChangedHandler(NotificationType notificationType, NotificationParams notificationParams)
        {
            bool state = (bool) notificationParams.Data;
            if (state == false)
            {
                _levelTimerCoroutine ??= StartCoroutine(GameplayTimer());
            }
            else
            {
                if (_levelTimerCoroutine != null)
                {
                    StopCoroutine(_levelTimerCoroutine);
                    _levelTimerCoroutine = null;
                }
            }
        }
        
        private IEnumerator SessionTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                _playerDataService.SessionTime++;
            }
        }

        private IEnumerator GameplayTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                _playerDataService.PlayTime++;
            }
        }
    }
}