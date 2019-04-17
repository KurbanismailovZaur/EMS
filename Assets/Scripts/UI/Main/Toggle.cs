using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace UI.Main
{
	public class Toggle : MonoBehaviour 
	{
        [Serializable]
        public class ToggledEvent : UnityEvent<bool> { }

        [SerializeField]
        private Image _checkImage;

        [SerializeField]
        private bool _state;

        public ToggledEvent Toggled;

        public bool State
        {
            get => _state;
            set
            {
                if (value == _state) return;

                _state = value;
                _checkImage.gameObject.SetActive(_state);

                Toggled.Invoke(_state);
            }
        }
        
        public void ToggleState() => State = !_state;
    }
}