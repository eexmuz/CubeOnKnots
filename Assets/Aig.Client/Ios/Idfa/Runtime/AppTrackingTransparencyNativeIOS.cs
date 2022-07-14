#if UNITY_IOS && !UNITY_EDITOR

namespace Aig.Client.Ios.Idfa
{
    public static class AppTrackingTransparencyNativeIOS
    {
        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern void RequestTrackingAuthorizationNative(string gameObjectNameCallback, string methodNameCallback, bool forceShow);
    }
}
#endif