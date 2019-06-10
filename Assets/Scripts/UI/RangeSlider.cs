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

        private string _valueSuffix;

        public string ValueSuffix
        {
            get => _valueSuffix;
            set
            {
                _valueSuffix = value;
                UpdateTexts();
            }
        }

        [Header("Events")]
        public ChangedEvent Changed;

        public float MinRange
        {
            get => _minSlider.minValue;
            set
            {
                if (value > MaxRange)
                    MaxRange = value;

                _minSlider.minValue = value;

                _eatEvents = true;

                var currentMaxValue = MaxValue;
                _maxSlider.minValue = value;
                MaxValue = currentMaxValue;

                if (currentMaxValue != MaxValue)
                    CallEvent();

                UpdateTexts();

                _eatEvents = false;
            }
        }

        public float MaxRange
        {
            get => _maxSlider.maxValue;
            set
            {
                if (value < MinRange)
                    MinRange = value;

                _minSlider.maxValue = value;

                _eatEvents = true;

                var currentMaxValue = MaxValue;
                _maxSlider.maxValue = value;
                MaxValue = currentMaxValue;

                if (currentMaxValue != MaxValue)
                    CallEvent();

                UpdateTexts();

                _eatEvents = false;
            }
        }

        public float MinValue
        {
            get => _minSlider.value;
            set
            {
                _minSlider.value = value;

                CheckMinChanging();
            }
        }

        public float MaxValue
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
            if (MaxValue < MinValue)
                MaxValue = MinValue;
        }

        private void CheckMaxChanging()
        {
            if (MaxValue < MinValue)
                MinValue = MaxValue;
        }

        private void UpdateTexts()
        {
            _minTextComponent.text = $"{MinValue} {ValueSuffix}";
            _maxTextComponent.text = $"{MaxValue} {ValueSuffix}";
        }

        private void CallEvent()
        {
            if (_eatEvents) return;

            UpdateTexts();

            Changed.Invoke(MinValue, MaxValue);
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