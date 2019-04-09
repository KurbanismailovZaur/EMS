using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.EventSystems;
using System;

namespace UI.Main
{
	public class Button : MonoBehaviour 
	{
        private Menu _menu;

        [SerializeField]
        private bool _hideContextOnClick;

        public event Action<Button> Pointed;

        private void Start() => _menu = GetComponentInParent<Menu>();

        #region Event handlers
        public void EventTrigger_PointerEnter(BaseEventData eventData) => Pointed?.Invoke(this);

        public void UnityEngine_UI_Button_OnClick()
        {
            if (_hideContextOnClick) _menu.HideCurrentGroupContext();
        }
        #endregion
    }
}