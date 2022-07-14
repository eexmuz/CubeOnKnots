using System.Collections;
using Core;
using Core.Settings;
using Core.Attributes;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertexDI.Game
{
    public class LoadingLogo : DIBehaviour
    {
        #region Fields

        public CanvasGroup canvasGroup;
        
        private float _loadingProgress;
        private float _currentWidth;

        [SerializeField]
        private RectTransform loadingProgressBar;

        [SerializeField]
        private float _minWidth;

        [SerializeField]
        private float _maxWidth;
        
        [SerializeField]
        [Range(1f, 100f)]
        private float loadingProgressBarSpeed;

        #endregion

        #region Public Methods and Operators

        public YieldInstruction FadeOut()
        {
            return canvasGroup.DOFade(0.0f, 0.33f).WaitForCompletion();
        }
        
        public void SetLoadingProgress(float progress)
        {
            _loadingProgress = progress;
            _loadingProgress = Mathf.Clamp01(_loadingProgress);
        }

        public IEnumerator WaitForProgress()
        {
            while (_currentWidth < _loadingProgress) yield return null;
        }

        #endregion

        #region Methods

        protected override void OnAppInitialized()
        {
            base.OnAppInitialized();

            _loadingProgress = 0.0f;
            _currentWidth = 0.0f;

            DontDestroyOnLoad(gameObject);
        }
        
        private void Update()
        {
            var delta = Time.unscaledDeltaTime * loadingProgressBarSpeed * 0.01f;

            var size = loadingProgressBar.sizeDelta;
            _currentWidth += delta;
            
            if (_currentWidth > _loadingProgress)
                _currentWidth = _loadingProgress;
            
            size.x = (_maxWidth - _minWidth) * _currentWidth + _minWidth;
            size.x = Mathf.Clamp(size.x, _minWidth, _maxWidth);

            loadingProgressBar.sizeDelta = size;
        }

        #endregion
    }
}