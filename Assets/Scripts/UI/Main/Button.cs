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
        public event Action<Button> Pointed;

        #region Event handlers
        public void EventTrigger_PointerEnter(BaseEventData eventData) => Pointed?.Invoke(this);
        #endregion
    }
}