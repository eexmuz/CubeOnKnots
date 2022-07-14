#if UNITY_ANDROID

using System;
using Aig.Client.Integration.Runtime.Settings;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace Aig.Client.Integration.Editor
{
    /// <summary>
    /// A pre processor that will run Integrations settings force validate.
    /// </summary>
    public class PreProcessor :
#if UNITY_2018_1_OR_NEWER
        IPreprocessBuildWithReport
#else
        IPreprocessBuild
#endif
    {
#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
#endif
        {
            Debug.Log("Pre Build Process started");
            var integrationSettings = Resources.Load<IntegrationSettings>("IntegrationSettings");

            if (integrationSettings == null)
            {
                throw new Exception("Integration Settings not created! Create it in context menu Azur/Integration/Integration Settings and put it to any Resources folder!");
            }

            integrationSettings.ForceValidate();
        }

        public int callbackOrder
        {
            get { return -1; }
        }
    }
}

#endif