using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace UI.Popups
{
    public class PopupManager : MonoSingleton<PopupManager>
    {
        [SerializeField]
        private Color _errorColor;

        [SerializeField]
        private Color _successColor;

        [SerializeField]
        private Popup _popupPrefab;
        
        private float _minOffsetFromBottom = 8;

        private float _maxOffsetFromBottom = 64;

        private RectTransform _rectTransform;

        private void Awake() => _rectTransform = GetComponent<RectTransform>();

        public void PopError(string message) => Pop(_errorColor, message);

        public void PopSuccess(string message) => Pop(_successColor, message);

        private void Pop(Color color, string message) => Instantiate(_popupPrefab, transform).StartAnimation(color, message);

        #region Event handlers
        public void Timeline_VisibilityChanged(bool state)
        {
            var pos = _rectTransform.anchoredPosition;
            pos.y = state ? _maxOffsetFromBottom : _minOffsetFromBottom;

            _rectTransform.anchoredPosition = pos;
        }
        #endregion
    }
}