using System.Collections.Generic;
using UnityEngine;

namespace Core.Services
{
    public interface ISoundService : IService
    {
        #region Public Properties

        float HighPitchRange { get; }

        float LowPitchRange { get; }

        #endregion

        #region Public Methods and Operators

        void FreeSource(AudioSource source);
        AudioSource GetSource(AudioClip clip);

        AudioSource PlaySoundPitchProgression(string soundName, int pitchProgression);
        
        AudioSource PlaySound(string soundName, float volume = 1f, bool loop = false, bool randomPitch = false, float pitch = 1f);
        
        AudioSource PlaySound(GameObject target, string soundName, float volume = 1f, bool randomPitch = false, float pitch = 1f);

        AudioSource PlaySound(GameObject target, AudioClip audioClip, float volume = 1f, bool randomPitch = false, float pitch = 1f);

        AudioSource PlaySound(AudioClip audioClip, float volume = 1f, bool loop = false, bool randomPitch = false, float pitch = 1f);
        

        void StopLoop(AudioSource source);

        void StopSound(AudioSource fxSource);

        #endregion
    }
}