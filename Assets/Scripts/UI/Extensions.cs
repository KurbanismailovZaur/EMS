using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace UI
{
	public static class Extensions
	{
        public static void SetRectFrom(this RectTransform rectTransform, RectTransform other)
        {
            rectTransform.pivot = other.pivot;
            rectTransform.anchorMin = other.anchorMin;
            rectTransform.anchorMax = other.anchorMax;
            rectTransform.anchoredPosition = other.anchoredPosition;
            rectTransform.sizeDelta = other.sizeDelta;
        }

        public static void Stretch(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}