#if UNITY_IOS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aig.Client.Ios.Idfa.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Aig.Client.Ios.Idfa.Editor
{
    public sealed class PostprocessBuildAddUserTrackingDescription : IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 999;

        private readonly string _localizationFolderName = "Localization";

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.iOS)
            {
                AddUserTrackingUsageDescription(report.summary.outputPath);

                AddLocalizationFiles(report.summary.outputPath);
            }
        }

        private void AddUserTrackingUsageDescription(string pathToBuiltProject)
        {
            var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            var plistDocument = new PlistDocument();

            plistDocument.ReadFromFile(plistPath);

            var localizations = GetLocalization();

            if (localizations.Count == 0)
            {
                Debug.LogError("Localization in AppTrackingSettings in not loaded!!!");
                return;
            }

            plistDocument.root.SetString("NSUserTrackingUsageDescription", localizations[0].description);

            plistDocument.WriteToFile(plistPath);
        }

        private void AddLocalizationFiles(string pathToBuiltProject)
        {
            var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
            var targetGuid = project.GetUnityMainTargetGuid();
#else
            var targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

            var localizations = GetLocalization();

            if (localizations.Count == 0)
            {
                Debug.LogError("Localization in AppTrackingSettings in not loaded!!!");
                return;
            }

            var projectLocalizationPath = Path.Combine(pathToBuiltProject, _localizationFolderName);

            foreach (var locale in localizations)
            {
                if(locale.IsEmpty()) continue;

                var localeDirName = $"{locale.langCode}.lproj";

                var dirPath = Path.Combine(projectLocalizationPath, localeDirName);

                if (Directory.Exists(dirPath))
                {
                    Directory.Delete(dirPath, true);
                }

                Directory.CreateDirectory(dirPath);

                var filePath = Path.Combine(dirPath, "InfoPlist.strings");

                //Write some text to the file
                var writer = new StreamWriter(filePath, true);

                writer.WriteLine($"\"NSUserTrackingUsageDescription\" = \"{locale.description}\";");

                writer.Close();

                var guid = project.AddFolderReference(dirPath, localeDirName);
                project.AddFileToBuild(targetGuid, guid);

                File.WriteAllText(projectPath, project.WriteToString());
            }
        }

        private List<LocalizedDescription> GetLocalization()
        {
            var foundAssetGUID = AssetDatabase.FindAssets($"t:{nameof(AppTrackingSettings)}").FirstOrDefault();

            if (foundAssetGUID == null)
            {
                return new List<LocalizedDescription>();
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(foundAssetGUID);
            var settings = AssetDatabase.LoadAssetAtPath<AppTrackingSettings>(assetPath);

            return settings.localizedDescriptionList;
        }
    }
}
#endif
