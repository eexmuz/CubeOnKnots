using System.Collections.Generic;
using UnityEngine;

namespace Core.Settings
{
    [CreateAssetMenu(menuName = "Settings/Sound Settings")]
    public class SoundSettings : ScriptableObject, ISettings
    {
        public List<AudioClip> AudioClips;
    }
}