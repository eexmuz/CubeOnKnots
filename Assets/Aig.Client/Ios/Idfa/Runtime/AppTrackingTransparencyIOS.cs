#if UNITY_IOS && !UNITY_EDITOR
namespace Aig.Client.Ios.Idfa
{
    public sealed class AppTrackingTransparencyIOS : IAppTrackingTransparency
    {
        void IAppTrackingTransparency.RequestTrackingAuthorization(TrackingAuthorizationDelegate callback, bool forceShow)
        {
            var gameObjectName = nameof(AppTrackingTransparencyBehaviour);
            var methodName = nameof(AppTrackingTransparencyBehaviour.OnTrackingAuthorizationStatusReceived);

            var appTracking = new UnityEngine.GameObject(gameObjectName).AddComponent<AppTrackingTransparencyBehaviour>();
            appTracking.SetCallback(callback);

            UnityEngine.Debug.Log("[AppTrackingTransparencyNative] RequestTrackingAuthorizationNative");

            AppTrackingTransparencyNativeIOS.RequestTrackingAuthorizationNative(gameObjectName, methodName, forceShow);
        }
    }
}
#endif