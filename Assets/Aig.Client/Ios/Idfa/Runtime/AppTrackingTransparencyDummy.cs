namespace Aig.Client.Ios.Idfa
{
    public sealed class AppTrackingTransparencyDummy : IAppTrackingTransparency
    {
        void IAppTrackingTransparency.RequestTrackingAuthorization(TrackingAuthorizationDelegate callback, bool forceShow)
        {
            callback.Invoke(TrackingAuthorizationStatus.NotDetermined);
        }
    }
}