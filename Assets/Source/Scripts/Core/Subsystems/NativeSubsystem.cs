// NativeSubsystem.cs
//
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//
// 
// -------------------------------------------------------------------------

using System.Collections.Generic;
using Core.Services;

namespace Core.Subsystems
{
    /// <summary>
    ///     Responsible for implementing Native OS functionality.
    /// </summary>
    public class NativeSubsystem : Subsystem
    {
        #region Monobehaviour

        private void Awake()
        {
            _inAppPurchaseService = gameObject.AddComponent<InAppPurchaseService>();
            _localNotificationService = gameObject.AddComponent<LocalNotificationService>();
            //_advertisementService = gameObject.AddComponent<AdvertisementService>();
            _playerPrefsService = gameObject.AddComponent<PlayerPrefsService>();
            _vibrationService = gameObject.AddComponent<VibrationService>();

            //_deepLinkService = gameObject.AddComponent<DeepLinkService>();

            Services = new List<IService>
            {
                _inAppPurchaseService,
                _localNotificationService,
                //_advertisementService,
                _playerPrefsService,
                _vibrationService
                //_deepLinkService
            };
        }

        #endregion

        #region Hidden members

        private IInAppPurchaseService _inAppPurchaseService;
        private ILocalNotificationService _localNotificationService;
        //private IAdvertisementService _advertisementService;
        private IPlayerPrefsService _playerPrefsService;
        private IVibrationService _vibrationService;
        //IDeepLinkService _deepLinkService;

        #endregion
    }
}
