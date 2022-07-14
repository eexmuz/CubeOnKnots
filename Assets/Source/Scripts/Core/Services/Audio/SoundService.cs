using System.Collections;
using System.Collections.Generic;
using Core.Settings;
using Core.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Services
{
    [InjectionAlias(typeof(ISoundService))]
    public class SoundService : Service, ISoundService
    {
        #region Fields

        private readonly List<AudioSource> _poolAudioSourcesBusy = new List<AudioSource>();
        private readonly List<AudioSource> _poolAudioSourcesFree = new List<AudioSource>();

        [UsedImplicitly]
        [Inject] private GameAudioSettings _gameAudioSettings;

        [Inject] private IGameOptionsService _gameOptionsService;

        [Inject] private SoundSettings _soundSettings;

        private bool _soundEnabled;
        private bool _musicEnabled;

        #endregion

        #region Public Properties

        public float HighPitchRange { get; } = 1.10f;
        
        public float LowPitchRange { get; } = .90f;

        #endregion

        #region Public Methods and Operators

        public void FreeSource(AudioSource source)
        {
            _poolAudioSourcesFree.Add(source);
            _poolAudioSourcesBusy.Remove(source);
        }

        public AudioSource GetSource(AudioClip clip)
        {
            var source = GetFreeAudioSouce();
            source.clip = clip;
            source.volume = 1.0f;
            source.pitch = 1.0f;
            return source;
        }

        public AudioSource PlaySoundPitchProgression(string soundName, int pitchProgression)
        {
            float pitch = 1f + (1f * pitchProgression) / 12f;
            
            return PlaySound(soundName, 1f, false, false, pitch);
        }

        public AudioSource PlaySound(string soundName, float volume = 1f, bool loop = false, bool randomPitch = false, float pitch = 1f)
        {
            var audioClip = GetPreloadedClip(soundName);

            if (audioClip == null) return null;
            /*
            if (IsEnabled == false)
            {
                return null;
            }
            */
            return PlaySound(audioClip, volume, loop, randomPitch, pitch);
        }

        public AudioSource PlaySound(GameObject target, string soundName, float volume = 1f, bool randomPitch = false, float pitch = 1f)
        {
            var audioClip = GetPreloadedClip(soundName);

            if (audioClip == null) return null;

            if (_soundEnabled == false) return null;

            var audioSourceGameObject = new GameObject("AudioSource");
            audioSourceGameObject.transform.position = target.transform.position;
            audioSourceGameObject.transform.rotation = target.transform.rotation;

            var fxSource = audioSourceGameObject.AddComponent<AudioSource>();

            fxSource.loop = false;
            fxSource.volume = volume;
            fxSource.clip = audioClip;
            fxSource.spatialBlend = 1;
            fxSource.minDistance = 5;
            fxSource.maxDistance = 300;
            fxSource.outputAudioMixerGroup = _gameAudioSettings.battleChannel;
            fxSource.rolloffMode = AudioRolloffMode.Linear;

            fxSource.pitch = randomPitch ? GetRandomPitch() : pitch;

            StartCoroutine(EndOfCustopClip_co(fxSource));

            fxSource.Play();

            return fxSource;
        }

        public AudioSource PlaySound(GameObject target, AudioClip audioClip, float volume = 1f, bool randomPitch = false, float pitch = 1f)
        {
            if (audioClip == null) return null;

            if (_soundEnabled == false) return null;

            var audioSourceGameObject = new GameObject("AudioSource");
            audioSourceGameObject.transform.position = target.transform.position;
            audioSourceGameObject.transform.rotation = target.transform.rotation;

            var fxSource = audioSourceGameObject.AddComponent<AudioSource>();

            fxSource.loop = false;
            fxSource.volume = volume;
            fxSource.clip = audioClip;
            fxSource.spatialBlend = 1;
            fxSource.minDistance = 5;
            fxSource.maxDistance = 300;
            fxSource.outputAudioMixerGroup = _gameAudioSettings.battleChannel;
            fxSource.rolloffMode = AudioRolloffMode.Linear;

            fxSource.pitch = randomPitch ? GetRandomPitch() : pitch;

            StartCoroutine(EndOfCustopClip_co(fxSource));

            fxSource.Play();

            return fxSource;
        }

        public AudioSource PlaySound(AudioClip audioClip, float volume = 1f, bool loop = false, bool randomPitch = false, float pitch = 1f)
        {
            if (audioClip == null) return null;

            if (_soundEnabled == false) return null;

            var fxSource = GetFreeAudioSouce();

            fxSource.loop = loop;
            fxSource.volume = volume;
            fxSource.clip = audioClip;
            fxSource.outputAudioMixerGroup = _gameAudioSettings.uiChannel;

            fxSource.pitch = randomPitch ? GetRandomPitch() : pitch;

            fxSource.Play();

            if (!loop) StartCoroutine(EndOfClip_co(fxSource));

            _poolAudioSourcesBusy.Add(fxSource);
            return fxSource;
        }

        public override void Run()
        {
            base.Run();

            Subscribe(NotificationType.ToggleSoundState, ToggleSoundStateHandler);
        }
        
        public void StopLoop(AudioSource source)
        {
            source.loop = false;
            StartCoroutine(EndOfClip_co(source));
        }

        public void StopSound(AudioSource fxSource)
        {
            fxSource.Stop();
            _poolAudioSourcesFree.Add(fxSource);

            if (_poolAudioSourcesBusy.Contains(fxSource)) _poolAudioSourcesBusy.Remove(fxSource);
        }

        #endregion

        #region Methods

        private void ToggleSoundStateHandler(NotificationType notificationType, NotificationParams notificationParams)
        {
            _soundEnabled = (bool) notificationParams.Data;
        }        

        private IEnumerator EndOfClip_co(AudioSource fxSource)
        {
            yield return new WaitForSeconds(fxSource.clip.length);

            if (!_poolAudioSourcesFree.Contains(fxSource)) _poolAudioSourcesFree.Add(fxSource);

            if (_poolAudioSourcesBusy.Contains(fxSource)) _poolAudioSourcesBusy.Remove(fxSource);
        }

        private IEnumerator EndOfCustopClip_co(AudioSource fxSource)
        {
            yield return new WaitForSeconds(fxSource.clip.length);

            if (fxSource != null && fxSource.gameObject != null) Destroy(fxSource.gameObject);
        }

        private AudioSource GetFreeAudioSouce()
        {
            AudioSource fxSource;

            if (_poolAudioSourcesFree.Count > 0)
            {
                fxSource = _poolAudioSourcesFree[0];
                _poolAudioSourcesBusy.Add(fxSource);
                _poolAudioSourcesFree.RemoveAt(0);
            }
            else
            {
                fxSource = gameObject.AddComponent<AudioSource>();
                _poolAudioSourcesBusy.Add(fxSource);
            }

            return fxSource;
        }

        private AudioClip GetPreloadedClip(string soundName)
        {
            var clip = _soundSettings.AudioClips.Find(audioClip => audioClip.name == soundName);
            return clip;
        }

        /* just in case
        public void RandomizeSFX(params string[] clips)
        {
            int randomIndex = UnityEngine.Random.Range(0, clips.Length);

            fxSource.pitch = getRandomPitch();
            fxSource.clip = LoadClip( clips[randomIndex] );
            fxSource.Play();
        }
        */

        private float GetRandomPitch()
        {
            return Random.Range(LowPitchRange, HighPitchRange);
        }

        #endregion
    }
}