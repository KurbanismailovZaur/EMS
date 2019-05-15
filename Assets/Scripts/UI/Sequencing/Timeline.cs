using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System.Globalization;
using System;
using UnityEngine.Events;

namespace UI.Sequencing
{
	public class Timeline : MonoSingleton<Timeline> 
	{
        [Serializable]
        public class ChangedEvent : UnityEvent<int> { }

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Slider _slider;

        [SerializeField]
        private Text _timeText;

        [SerializeField]
        private Button _playButton;

        [SerializeField]
        private GameObject _playImage;

        [SerializeField]
        private GameObject _pauseImage;

        private const float _timeDelta = 1f / 36f;

        private static NumberFormatInfo _nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

        private Coroutine _playRoutine;

        public ChangedEvent Changed;

        private void Start()
        {
            _slider.onValueChanged.AddListener(Slider_OnValueChanged);
            _playButton.onClick.AddListener(PlayButton_OnClick);
        }

        public void Show()
        {
            SetCanvasGroupParameters(1f, true);
        }

        public void Hide()
        {
            SetCanvasGroupParameters(0f, false);
        }

        private void SetCanvasGroupParameters(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }

        public void Play()
        {
            SetPlayImageStateTo(false);
            _playRoutine = StartCoroutine(PlayRoutineRoutine());
        }

        private IEnumerator PlayRoutineRoutine()
        {
            var startTime = Time.time - _slider.value * _timeDelta;

            while (true)
            {
                var time = Mathf.Round(((Time.time - startTime) % 1f).Remap(0f, 1f, 0f, 36f));

                _slider.value = time;

                yield return null;
            }
        }

        public void Pause()
        {
            if (_playRoutine == null) return;

            StopCoroutine(_playRoutine);
            _playRoutine = null;

            SetPlayImageStateTo(true);
        }

        private void SetPlayImageStateTo(bool state)
        {
            _playImage.SetActive(state);
            _pauseImage.SetActive(!state);
        }

        public void ResetState() => _slider.value = 0;

        #region Event handlers
        private void Slider_OnValueChanged(float value)
        {
            _timeText.text = $"{value}/36";

            Changed.Invoke((int)value);
        }

        private void PlayButton_OnClick()
        {
            if (_playRoutine == null)
                Play();
            else
                Pause();
        }
        #endregion
    }
}