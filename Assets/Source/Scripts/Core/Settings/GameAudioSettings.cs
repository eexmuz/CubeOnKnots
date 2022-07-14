using UnityEngine;
using UnityEngine.Audio;

namespace Core.Settings
{
    [CreateAssetMenu(menuName = "Settings/Audio Settings")]
    public class GameAudioSettings : ScriptableObject, ISettings
    {
        #region Fields

        public AudioMixer audioMixer;
        public AudioMixerGroup battleChannel;

        public AudioMixerGroup master;

        public AudioMixerGroup musicChannel;
        public AudioMixerGroup soundChannel;
        public AudioMixerGroup uiChannel;

        #endregion
    }
}