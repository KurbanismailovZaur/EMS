using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;

namespace UI.Referencing
{
	public class Column : MonoBehaviour 
	{
        [Serializable]
        public class RectTransformChangedEvent : UnityEvent<Vector2> { }

        public RectTransformChangedEvent RectTransformChanged;

        private void OnRectTransformDimensionsChange() => RectTransformChanged.Invoke(((RectTransform)transform).sizeDelta);
    }
}