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

        public RangeSlider RangeSlider => _slider;

        public void Show() => SetCanvasGroupOptions(1f, true);

        public void Hide() => SetCanvasGroupOptions(0f, false);

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