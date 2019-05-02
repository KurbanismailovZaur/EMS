using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace UI.Tables
{
	public class Header : MonoBehaviour 
	{
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private RectTransform _content;

        private void LateUpdate()
        {
            var anchoredPosition = _rectTransform.anchoredPosition;
            anchoredPosition.x = _content.anchoredPosition.x;

            _rectTransform.anchoredPosition = anchoredPosition;
        }
    }
}