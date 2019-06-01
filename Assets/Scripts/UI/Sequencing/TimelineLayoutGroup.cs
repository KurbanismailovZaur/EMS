using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;

namespace UI.Sequencing
{
	public class TimelineLayoutGroup : MonoBehaviour, ILayoutGroup
	{
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private RectTransform[] _points;
        
        private void Calculate()
        {
            var delta = _rectTransform.rect.width / (_points.Length - 1);

            for (int i = 0; i < _points.Length; i++)
            {
                var pos = _points[i].anchoredPosition;
                pos.x = delta * i;
                _points[i].anchoredPosition = pos;
            }
        }

        public void SetLayoutHorizontal()
        {
            Calculate();
        }

        public void SetLayoutVertical() { }

        private void OnRectTransformDimensionsChange()
        {
            Calculate();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Calculate();
        }
#endif
    }
}