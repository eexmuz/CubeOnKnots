using System.Collections;
using System.Collections.Generic;
using Core.Settings;
using Core.Attributes;
using UnityEngine;

namespace Core.Services
{
    [InjectionAlias(typeof(IMusicService))]
    public class MusicService : Service, IMusicService
    {
        #region Fields

        [Inject] private GameAudioSettings _gameAudioSettings;

        [Inject] private IGameOptionsService _gameOptionsService;
        
        [Inject] private SoundSettings _soundSettings;
        
        private bool _isEnabled = true;

        private bool _musicState;

        private AudioSource _musicSource;

        #endregion

        #region Public Properties

        public bool IsEnabled
        {
            get => true;

            set
            {
                _isEnabled = value;
                if (_isEnabled)
                {
                    _musicSource.Play();
                }
                else
                {
                    _musicSource.Pause();
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public void SetMusicState(bool state)
        {
            if (state == _musicState)
            {
                return;
            }

            if (state == false)
            {
                _musicState = false;
                StartCoroutine(StopMusic_co());
                return;
            }
            
            var clip = GetPreloadedClip(Sounds.Music);
            
            if (clip == null) return;
            
            StartCoroutine(PlayMusic_co(clip));
            _musicState = true;
        }

        public override void Run()
        {
            base.Run();
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.outputAudioMixerGroup = _gameAudioSettings.musicChannel;
            
            Subscribe(NotificationType.ToggleMusicState, OnToggleMusicState);
        }

        private void OnToggleMusicState(NotificationType notificationType, NotificationParams notificationParams)
        {
            bool state = (bool) notificationParams.Data;
            SetMusicState(state);
        }

        #endregion

        #region Methods

        private AudioClip GetPreloadedClip(string soundName)
        {
            var clip = _soundSettings.AudioClips.Find(audioClip => audioClip.name == soundName);
            return clip;
        }

        private IEnumerator PlayMusic_co(AudioClip clip)
        {
            yield return StopMusic_co();

            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.volume = 0;

            _musicSource.Play();

            if (!_isEnabled) _musicSource.Pause();

            for (float v = 0; v <= 1f; v += 0.1f)
            {
                _musicSource.volume = v;
                yield return null;
            }
        }

        private IEnumerator StopMusic_co()
        {
            for (var v = _musicSource.volume; v > 0; v -= 0.1f)
            {
                _musicSource.volume = v;
                yield return null;
            }

            _musicSource.Stop();
        }

        #endregion
    }
}