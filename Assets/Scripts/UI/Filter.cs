using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Exceptions;

namespace UI
{
	public class Filter : MonoBehaviour 
	{
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private RangeSlider _slider;

        private bool _isOpen;

        public void Show()
        {
            if (_isOpen) throw new BusyException("Already opened.");

            _isOpen = true;

            SetCanvasGroupOptions(1f, true);
        }

        public void Hide()
        {
            if (!_isOpen) throw new BusyException("Already closed.");

            _isOpen = false;

            SetCanvasGroupOptions(0f, false);
        }

        private void SetCanvasGroupOptions(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }

        public void SetRanges(float min, float max)
        {
            _slider.MinRange = min;
            _slider.MaxRange = max;
        }

        public void ResetValues()
        {
            _slider.MinValue = _slider.MinRange;
            _slider.MaxValue = _slider.MaxRange;
        }
	}
}