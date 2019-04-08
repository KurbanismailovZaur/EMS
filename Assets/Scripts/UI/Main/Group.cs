using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Main
{
    public class Group : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _context;

        private Group _currentGroup;

        private bool _isShowed;

        public event Action<Group> Showed;

        public event Action<Group> Hided;

        private Panel _panel;

        private void Start()
        {
            _panel = GetComponentInParent<Panel>();

            var contextTransform = _context.transform;

            for (int i = 0; i < contextTransform.childCount; i++)
            {
                var child = contextTransform.GetChild(i);

                Group group = child.GetComponent<Group>();
                Button button = child.GetComponent<Button>();

                if (group)
                {
                    group.Showed += Group_Showed;
                    group.Hided += Group_Hided;

                }
                else if (button)
                {
                    button.Pointed += Button_Pointed;
                }
            }
        }

        public void ShowContext()
        {
            if (_isShowed) return;

            _isShowed = true;

            SetContextParameters(1f, true);
            Showed?.Invoke(this);
        }

        public void HideContext()
        {
            if (!_isShowed) return;

            _isShowed = false;

            if (_currentGroup)
            {
                _currentGroup.HideContext();
                _currentGroup = null;
            }

            SetContextParameters(0f, false);
            Hided?.Invoke(this);
        }

        private void SetContextParameters(float alpha, bool blockRaycast)
        {
            _context.blocksRaycasts = blockRaycast;
            _context.alpha = alpha;
        }

        #region Event handlers
        private void Group_Showed(Group group)
        {
            _currentGroup?.HideContext();
            _currentGroup = group;
        }

        private void Group_Hided(Group group) => _currentGroup = null;

        private void Button_Pointed(Button button) => _currentGroup?.HideContext();

        public void EventTrigger_PointerEnter(BaseEventData eventData)
        {
            if (!_panel.UseMouseEnterAsClick) return;

            ShowContext();
        }
        #endregion
    }
}