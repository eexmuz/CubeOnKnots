using System;

using UnityEngine;

namespace Aig.Client.Ios.Idfa
{
    public sealed class AppTrackingTransparencyBehaviour : MonoBehaviour
    {
        private TrackingAuthorizationDelegate _callback;

        public void OnTrackingAuthorizationStatusReceived(string status)
        {
            try
            {
                if (status == null)
                {
                    throw new ArgumentNullException(nameof(status));
                }

                if (_callback == null)
                {
                    throw new ArgumentNullException(nameof(_callback));
                }

                var parsedStatus = (TrackingAuthorizationStatus)Enum.Parse(typeof(TrackingAuthorizationStatus), status);

                _callback.Invoke(parsedStatus);
            }
            finally
            {
                Destroy(gameObject);
            }
        }

        public void SetCallback(TrackingAuthorizationDelegate callback)
        {
            _callback = callback;
        }
    }
}