namespace Aig.Client.Ios.Idfa
{
    public interface IAppTrackingTransparency
    {
        void RequestTrackingAuthorization(TrackingAuthorizationDelegate callback, bool forceShow);
    }
}