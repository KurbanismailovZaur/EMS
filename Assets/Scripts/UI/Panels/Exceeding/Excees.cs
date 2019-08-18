using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;

namespace UI.Panels.Exceeding
{
	public class Excees : MonoBehaviour 
	{
        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private Image _visibilityImage;

        [Header("Colors")]
        [SerializeField]
        private Color _uncheckedColor;

        [SerializeField]
        private Color _checkedColor;

        public Action<bool> Changed;

        public bool IsChecked { get; private set; }

        public string Name { get => _nameText.text; set => _nameText.text = value; }

        public void SetVisibility(bool state, bool callEvent = true)
        {
            IsChecked = state;
            _visibilityImage.color = state ? _checkedColor : _uncheckedColor;

            if (callEvent)
                Changed?.Invoke(IsChecked);
        }

        public void Button_OnClick() => SetVisibility(!IsChecked);
    }
}