using System.Collections.Generic;
using UnityEngine;

namespace Aig.Client.Ios.Idfa.Editor
{
    [CreateAssetMenu(
        fileName = nameof(AppTrackingSettings),
        menuName = "Azur/iOS/App Tracking Settings",
        order    = 9999)]
    public sealed class AppTrackingSettings : ScriptableObject
    {
        [Space(10)]
        public string googleDocId = "1awP3jSe8cZZ7ZKBz3xzznzQWw9lIH7PBzl-4dV-ClDU";
        //codes according to https://www.ibabbleon.com/iOS-Language-Codes-ISO-639.html
        public List<LocalizedDescription> localizedDescriptionList = new List<LocalizedDescription>();
    }
}