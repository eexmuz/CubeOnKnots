namespace Core.Services
{
    public interface IVibrationService : IService
    {
        void VibrateLight();
        void VibrateMedium();
        void VibrateHeavy();
    }
}
