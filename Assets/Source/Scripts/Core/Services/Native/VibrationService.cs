using Core.Attributes;
using MoreMountains.NiceVibrations;

namespace Core.Services
{
    [InjectionAlias(typeof(IVibrationService))]
    public class VibrationService : Service, IVibrationService
    {
        [Inject] private IGameOptionsService _gameOptionsService;

        public void VibrateLight()
        {
#if UNITY_IOS
            Vibrate (HapticTypes.LightImpact);
#endif

#if UNITY_ANDROID
            Vibrate(HapticTypes.LightImpact);
#endif
        }

        public void VibrateMedium()
        {
#if UNITY_IOS
            Vibrate (HapticTypes.MediumImpact);
#endif

#if UNITY_ANDROID
            Vibrate(HapticTypes.MediumImpact);
#endif
        }

        public void VibrateHeavy()
        {
#if UNITY_IOS
            Vibrate (HapticTypes.HeavyImpact);
#endif

#if UNITY_ANDROID
            Vibrate(HapticTypes.HeavyImpact);
#endif
        }

        private void Vibrate(HapticTypes type)
        {
            if (_gameOptionsService.Vibration == false)
                return;

            MMVibrationManager.Haptic(type, true);
        }
    }
}
