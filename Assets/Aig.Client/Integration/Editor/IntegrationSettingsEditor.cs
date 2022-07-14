using System;
using System.Collections.Generic;
using System.Linq;
using Aig.Client.Integration.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Aig.Client.Integration.Editor
{
    [CustomEditor(typeof(IntegrationSettings))]
    public class IntegrationSettingsEditor : UnityEditor.Editor
    {
        private IntegrationSettings _settings;

        private WWW _www;

        private SerializedProperty googleDocId;

        private SerializedProperty autoRun;

        private SerializedProperty enableAnalytics;

        private SerializedProperty enableAppMetrica;

        private SerializedProperty appMetricaApiKey;
        private SerializedProperty appMetricaSessionTimeoutSec;

        private SerializedProperty enableAppsFlyer;

        private SerializedProperty appsFlyerDevKey;
        private SerializedProperty appsFlyerAppId;
        private SerializedProperty appsFlyerSessionTimeoutSec;

        private SerializedProperty enableAds;

        private SerializedProperty maxSdkKey;
        private SerializedProperty adMobAndroidAppId;
        private SerializedProperty adMobIosAppId;

        private SerializedProperty androidInterstitialAdUnitId;
        private SerializedProperty androidRewardedAdUnitId;
        private SerializedProperty androidBannerAdUnitId;

        private SerializedProperty iosInterstitialAdUnitId;
        private SerializedProperty iosRewardedAdUnitId;
        private SerializedProperty iosBannerAdUnitId;

        private SerializedProperty interstitialDelayAndroid;
        private SerializedProperty firstInterstitialDelayAndroid;

        private SerializedProperty interstitialDelayIos;
        private SerializedProperty firstInterstitialDelayIos;

        private SerializedProperty loadInterstitialOnStart;
        private SerializedProperty loadRewardedOnStart;
        private SerializedProperty createBannerOnStart;
        private SerializedProperty autoShowBannerOnStart;
        private SerializedProperty autoShowBannerDelay;

        private SerializedProperty locationUsageDescription;

        //private SerializedProperty consentFlowEnabled;
        //private SerializedProperty consentFlowPrivacyPolicyUrl;

        void OnEnable()
        {
            _settings = (IntegrationSettings) target;

            googleDocId = serializedObject.FindProperty("googleDocId");

            autoRun = serializedObject.FindProperty("autoRun");

            enableAnalytics = serializedObject.FindProperty("enableAnalytics");

            enableAppMetrica = serializedObject.FindProperty("enableAppMetrica");

            appMetricaApiKey = serializedObject.FindProperty("appMetricaApiKey");
            appMetricaSessionTimeoutSec = serializedObject.FindProperty("appMetricaSessionTimeoutSec");

            enableAppsFlyer = serializedObject.FindProperty("enableAppsFlyer");

            appsFlyerDevKey = serializedObject.FindProperty("appsFlyerDevKey");
            appsFlyerAppId = serializedObject.FindProperty("appsFlyerAppId");
            appsFlyerSessionTimeoutSec = serializedObject.FindProperty("appsFlyerSessionTimeoutSec");

            enableAds = serializedObject.FindProperty("enableAds");

            maxSdkKey = serializedObject.FindProperty("maxSdkKey");
            adMobAndroidAppId = serializedObject.FindProperty("adMobAndroidAppId");
            adMobIosAppId = serializedObject.FindProperty("adMobIosAppId");

            androidInterstitialAdUnitId = serializedObject.FindProperty("androidInterstitialAdUnitId");
            androidRewardedAdUnitId = serializedObject.FindProperty("androidRewardedAdUnitId");
            androidBannerAdUnitId = serializedObject.FindProperty("androidBannerAdUnitId");

            iosInterstitialAdUnitId = serializedObject.FindProperty("iosInterstitialAdUnitId");
            iosRewardedAdUnitId = serializedObject.FindProperty("iosRewardedAdUnitId");
            iosBannerAdUnitId = serializedObject.FindProperty("iosBannerAdUnitId");

            interstitialDelayAndroid = serializedObject.FindProperty("interstitialDelayAndroid");
            firstInterstitialDelayAndroid = serializedObject.FindProperty("firstInterstitialDelayAndroid");

            interstitialDelayIos = serializedObject.FindProperty("interstitialDelayIos");
            firstInterstitialDelayIos = serializedObject.FindProperty("firstInterstitialDelayIos");

            loadInterstitialOnStart = serializedObject.FindProperty("loadInterstitialOnStart");
            loadRewardedOnStart = serializedObject.FindProperty("loadRewardedOnStart");
            createBannerOnStart = serializedObject.FindProperty("createBannerOnStart");
            autoShowBannerOnStart = serializedObject.FindProperty("autoShowBannerOnStart");
            autoShowBannerDelay = serializedObject.FindProperty("autoShowBannerDelay");

            locationUsageDescription = serializedObject.FindProperty("locationUsageDescription");

            //consentFlowEnabled = serializedObject.FindProperty("consentFlowEnabled");
            //consentFlowPrivacyPolicyUrl = serializedObject.FindProperty("consentFlowPrivacyPolicyUrl");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            var intent = EditorGUI.indentLevel;

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.PropertyField(googleDocId);

                GUI.enabled = googleDocId.stringValue.Length > 0;
                if (GUILayout.Button("Load"))
                {
                    LoadIntegrationSettings();
                }

                GUI.enabled = true;

                //General
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(autoRun);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();

                //Analytics
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(enableAnalytics);

                    EditorGUI.indentLevel++;

                    GUI.enabled = enableAnalytics.boolValue;

                    //AppMetrica
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.PropertyField(enableAppMetrica);

                        GUI.enabled = enableAppMetrica.boolValue && enableAnalytics.boolValue;
                        EditorGUILayout.PropertyField(appMetricaApiKey);
                        EditorGUILayout.PropertyField(appMetricaSessionTimeoutSec);
                        GUI.enabled = enableAnalytics.boolValue;
                    }
                    EditorGUILayout.EndVertical();

                    GUI.enabled = enableAnalytics.boolValue;

                    //AppsFlyer
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.PropertyField(enableAppsFlyer);

                        GUI.enabled = enableAppsFlyer.boolValue && enableAnalytics.boolValue;
                        EditorGUILayout.PropertyField(appsFlyerDevKey);
                        EditorGUILayout.PropertyField(appsFlyerAppId);
                        EditorGUILayout.PropertyField(appsFlyerSessionTimeoutSec);
                        GUI.enabled = enableAnalytics.boolValue;
                    }
                    EditorGUILayout.EndVertical();

                    GUI.enabled = true;

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);
                //Ads
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.PropertyField(enableAds);

                    EditorGUI.indentLevel++;

                    //AppLovin Max
                    EditorGUILayout.BeginVertical("box");
                    {
                        GUI.enabled = enableAds.boolValue;
                        EditorGUILayout.PropertyField(maxSdkKey);

                        EditorGUILayout.PropertyField(adMobAndroidAppId);
                        EditorGUILayout.PropertyField(adMobIosAppId);

                        EditorGUILayout.PropertyField(androidInterstitialAdUnitId);
                        EditorGUILayout.PropertyField(androidRewardedAdUnitId);
                        EditorGUILayout.PropertyField(androidBannerAdUnitId);

                        EditorGUILayout.PropertyField(iosInterstitialAdUnitId);
                        EditorGUILayout.PropertyField(iosRewardedAdUnitId);
                        EditorGUILayout.PropertyField(iosBannerAdUnitId);

                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(interstitialDelayAndroid);
                        EditorGUILayout.PropertyField(firstInterstitialDelayAndroid);
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(interstitialDelayIos);
                        EditorGUILayout.PropertyField(firstInterstitialDelayIos);
                        GUILayout.Space(10);

                        EditorGUILayout.PropertyField(loadInterstitialOnStart);
                        EditorGUILayout.PropertyField(loadRewardedOnStart);
                        EditorGUILayout.PropertyField(createBannerOnStart);
                        EditorGUILayout.PropertyField(autoShowBannerOnStart);
                        EditorGUILayout.PropertyField(autoShowBannerDelay);

                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel = intent;

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("IOS only");
                EditorGUILayout.PropertyField(locationUsageDescription);
                GUILayout.Space(10);
                //EditorGUILayout.PropertyField(consentFlowEnabled);
                //EditorGUILayout.PropertyField(consentFlowPrivacyPolicyUrl);
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Force Validation"))
            {
                var target = (IntegrationSettings) serializedObject.targetObject;
                target.ForceValidate();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_settings);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void LoadIntegrationSettings()
        {
            //_www = new WWW($"https://docs.google.com/spreadsheets/d/{googleDocId.stringValue}/gviz/tq?tqx=out:csv&sheet=texts");
            _www = new WWW($"https://docs.google.com/spreadsheets/d/{googleDocId.stringValue}/export?format=csv");

            EditorApplication.update += Tick;
        }

        private void Tick()
        {
            if (_www.isDone)
            {
                if (!string.IsNullOrEmpty(_www.error))
                {
                    Debug.LogError("WWW failed: " + _www.error);
                }
                else
                {
                    ParseLocalizationCsv(_www.text);
                }

                EditorApplication.update -= Tick;

                _www.Dispose();
            }
        }

        private void ParseLocalizationCsv(string text)
        {
            var csvData = text;

            var fieldsList = new Dictionary<string, string>();

            // convert text into rows by splitting along line breaks
            var csvRows = csvData.Split("\n"[0]).ToList();

            for (var i = 0; i < csvRows.Count; i++)
            {
                var row = csvRows[i];

                if (row[row.Length - 1] == '\r')
                {
                    row = row.Substring(0, row.Length - 1);
                }

                var columns = row.Split(new[] {","}, StringSplitOptions.None);

                if (string.IsNullOrEmpty(columns[0]) == false)
                {
                    fieldsList.Add(columns[0], columns[1]);
                }
            }

            appMetricaApiKey.stringValue = fieldsList["appMetricaApiKey"];

            if (string.IsNullOrEmpty(fieldsList["appMetricaSessionTimeoutSec"]) == false)
            {
                appMetricaSessionTimeoutSec.intValue = int.Parse(fieldsList["appMetricaSessionTimeoutSec"]);
            }

            appsFlyerDevKey.stringValue = fieldsList["appsFlyerDevKey"];
            appsFlyerAppId.stringValue = fieldsList["appsFlyerAppId"];

            if (string.IsNullOrEmpty(fieldsList["appsFlyerSessionTimeoutSec"]) == false)
            {
                appsFlyerSessionTimeoutSec.intValue = int.Parse(fieldsList["appsFlyerSessionTimeoutSec"]);
            }

            maxSdkKey.stringValue = fieldsList["maxSdkKey"];
            adMobAndroidAppId.stringValue = fieldsList["adMobAndroidAppId"];
            adMobIosAppId.stringValue = fieldsList["adMobIosAppId"];

            androidInterstitialAdUnitId.stringValue = fieldsList["androidInterstitialAdUnitId"];
            androidRewardedAdUnitId.stringValue = fieldsList["androidRewardedAdUnitId"];
            androidBannerAdUnitId.stringValue = fieldsList["androidBannerAdUnitId"];

            iosInterstitialAdUnitId.stringValue = fieldsList["iosInterstitialAdUnitId"];
            iosRewardedAdUnitId.stringValue = fieldsList["iosRewardedAdUnitId"];
            iosBannerAdUnitId.stringValue = fieldsList["iosBannerAdUnitId"];

            if (string.IsNullOrEmpty(fieldsList["interstitialDelayAndroid"]) == false)
            {
                interstitialDelayAndroid.intValue = int.Parse(fieldsList["interstitialDelayAndroid"]);
            }

            if (string.IsNullOrEmpty(fieldsList["firstInterstitialDelayAndroid"]) == false)
            {
                firstInterstitialDelayAndroid.intValue = int.Parse(fieldsList["firstInterstitialDelayAndroid"]);
            }

            if (string.IsNullOrEmpty(fieldsList["interstitialDelayIos"]) == false)
            {
                interstitialDelayIos.intValue = int.Parse(fieldsList["interstitialDelayIos"]);
            }

            if (string.IsNullOrEmpty(fieldsList["firstInterstitialDelayIos"]) == false)
            {
                firstInterstitialDelayIos.intValue = int.Parse(fieldsList["firstInterstitialDelayIos"]);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
