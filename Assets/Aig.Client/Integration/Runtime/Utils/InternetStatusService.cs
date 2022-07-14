using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Aig.Client.Integration.Runtime.Utils
{
    public class InternetStatusService : MonoBehaviour
    {
        public static InternetStatusService Instance { get; private set; }

        private const string pingAddress = "8.8.8.8"; // Google Public DNS server
        private const float poolingRate = 3.0f;
        private Ping ping;
        private float pingStartTime;

        private bool _isConnected;

        public bool IsConnectedToInternet
        {
            get
            {
                return _isConnected;
            }
            private set
            {
                if (_isConnected == value)
                {
                    return;
                }

                _isConnected = value;
                Debug.Log("Connection Status Changed: " + value);
                OnConnectionStatusChange?.Invoke(value);
            }
        }

        public event Action<bool> OnConnectionStatusChange;

        void Awake()
        {
            //Singleton
            if (Instance != null)
            {
                Debug.LogError("InternetStatus already existed!");
                Destroy(Instance);
            }

            Instance = this;

            StartCoroutine(MonitorInternetConnection());
        }

        private IEnumerator MonitorInternetConnection()
        {
            while (true)
            {
                yield return StartCoroutine(CheckInternetConnection());
                yield return new WaitForSeconds(poolingRate);
            }
        }

        private IEnumerator CheckInternetConnection()
        {
            yield return new WaitForEndOfFrame();

            ping = new Ping(pingAddress);
            pingStartTime = Time.time;

            bool internetPossiblyAvailable;
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    internetPossiblyAvailable = true;
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    internetPossiblyAvailable = true;
                    break;
                default:
                    internetPossiblyAvailable = false;
                    break;
            }

            if (internetPossiblyAvailable == false)
            {
                IsConnectedToInternet = false;
                yield break;
            }

            while (true)
            {
                var stopCheck = true;
                if (ping.isDone)
                {
                    IsConnectedToInternet = true;
                }
                else if (Time.time - pingStartTime < poolingRate)
                {
                    stopCheck = false;
                }
                else
                {
                    IsConnectedToInternet = false;
                }

                if (stopCheck)
                {
                    ping = null;
                    break;
                }

                yield return null;
            }
        }
    }
}