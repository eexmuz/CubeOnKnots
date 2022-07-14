using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;

namespace Aig.Client.Ios.Idfa
{
    public static class AppTrackingTransparency
    {
        private static readonly IAppTrackingTransparency appTracking = ResolveAppTrackingTransparency();

        public static void RequestTrackingAuthorization(TrackingAuthorizationDelegate callback, bool forceShow = false)
        {
            appTracking.RequestTrackingAuthorization(callback, forceShow);
        }

        public static Task<TrackingAuthorizationStatus> RequestTrackingAuthorizationAsync(bool forceShow = false)
        {
            var completionSource = new TaskCompletionSource<TrackingAuthorizationStatus>();

            void OnTrackingAuthorizationStatusReceived(TrackingAuthorizationStatus status)
            {
                completionSource.TrySetResult(status);
            }

            try
            {
                appTracking.RequestTrackingAuthorization(OnTrackingAuthorizationStatusReceived, forceShow);
            }
            catch (Exception exception)
            {
                completionSource.SetException(exception);
            }

            return completionSource.Task;
        }

        private static IAppTrackingTransparency ResolveAppTrackingTransparency()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new AppTrackingTransparencyIOS();
#endif
            return new AppTrackingTransparencyDummy();
        }
    }
}