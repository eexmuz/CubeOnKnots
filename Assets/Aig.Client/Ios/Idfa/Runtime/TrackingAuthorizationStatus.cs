namespace Aig.Client.Ios.Idfa
{
    public enum TrackingAuthorizationStatus
    {
        /// <summary>
        /// User has not yet received an authorization request
        /// to authorize access to app-related data that can be used
        /// for tracking the user or the device.
        /// </summary>
        NotDetermined,
        /// <summary>
        /// Authorization to access app-related data that can be used
        /// for tracking the user or the device is restricted.
        /// </summary>
        Restricted,
        /// <summary>
        /// User denies authorization to access app-related data that can be used for tracking the user or the device.
        /// </summary>
        Denied,
        /// <summary>
        /// User authorizes access to app-related data that can be used
        /// for tracking the user or the device.
        /// </summary>
        Authorized,
    }
}