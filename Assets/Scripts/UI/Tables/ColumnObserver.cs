﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;

namespace UI.Tables
{
    public class ColumnObserver : MonoBehaviour
    {
        [SerializeField]
        private LayoutElement _element;

        [SerializeField]
        private float _personalXOffset = 0f;

        #region Event handlers
        public void Column_RectTransformChanged(Vector2 sizeDelta)
        {
            _element.preferredWidth = sizeDelta.x + _personalXOffset;
        }
        #endregion
    }
}