using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace UI
{
    public class RangeSlider : MonoBehaviour
    {
        #region Class-events
        [Serializable]
        public class ChangedEvent : UnityEvent<float, float> { }
        #endregion

        [SerializeField]
        private Slider _minSlider;

        [SerializeField]
        private Text _minTextComponent;

        [SerializeField]
        private Slider _maxSlider;

        [SerializeField]
        private Text _maxTextComponent;

        private bool _eatEvents;

        [Header("Events")]
        public ChangedEvent Changed;

        public float MinValue
        {
            get => _minSlider.minValue;
            set
            {
                if (value > MaxValue)
                    MaxValue = value;

                _minSlider.minValue = value;

                _eatEvents = true;

                var currentMaxValue = CurrentMaxValue;
                _maxSlider.minValue = value;
                CurrentMaxValue = currentMaxValue;

                if (currentMaxValue != CurrentMaxValue)
                    CallEvent();

                _eatEvents = false;
            }
        }

        public float MaxValue
        {
            get => _maxSlider.maxValue;
            set
            {
                if (value < MinValue)
                    MinValue = value;

                _minSlider.maxValue = value;

                _eatEvents = true;

                var currentMaxValue = CurrentMaxValue;
                _maxSlider.maxValue = value;
                CurrentMaxValue = currentMaxValue;

                if (currentMaxValue != CurrentMaxValue)
                    CallEvent();

                _eatEvents = false;
            }
        }

        public float CurrentMinValue
        {
            get => _minSlider.value;
            set
            {
                _minSlider.value = value;

                CheckMinChanging();
            }
        }

        public float CurrentMaxValue
        {
            get => _maxSlider.minValue + _maxSlider.maxValue - _maxSlider.value;
            set
            {
                _maxSlider.value = _maxSlider.minValue + _maxSlider.maxValue - value;

                CheckMaxChanging();
            }
        }

        private void Start()
        {
            _minSlider.onValueChanged.AddListener(MinSlider_OnValueChanged);
            _maxSlider.onValueChanged.AddListener(MaxSlider_OnValueChanged);
        }

        private void CheckMinChanging()
        {
            if (CurrentMaxValue < CurrentMinValue)
                CurrentMaxValue = CurrentMinValue;
        }

        private void CheckMaxChanging()
        {
            if (CurrentMaxValue < CurrentMinValue)
                CurrentMinValue = CurrentMaxValue;
        }

        private void UpdateTexts()
        {
            _minTextComponent.text = CurrentMinValue.ToString();
            _maxTextComponent.text = CurrentMaxValue.ToString();
        }

        private void CallEvent()
        {
            if (_eatEvents) return;

            UpdateTexts();

            Changed.Invoke(CurrentMinValue, CurrentMaxValue);
        }

        #region Event handlers
        private void MinSlider_OnValueChanged(float value)
        {
            CheckMinChanging();

            CallEvent();
        }

        private void MaxSlider_OnValueChanged(float value)
        {
            CheckMaxChanging();

            CallEvent();
        }
        #endregion
    }
}