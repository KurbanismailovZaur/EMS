using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace UI.Referencing
{
	public class Cell : MonoBehaviour 
	{
        public enum Type
        {
            Int,
            String,
            NullableString
        }

        [Serializable]
        public class ClickedEvent : UnityEvent<Cell> { }

        [SerializeField]
        private Text _text;

        [SerializeField]
        private Type _type;

        [Header("Events")]
        public ClickedEvent Clicked;

        public Text Text => _text;

        public Type CellType => _type;

        #region Event handlers
        public void Button_OnClick() => Clicked.Invoke(this);
        #endregion
    }
}