using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

namespace UI.Tables
{
    public class Tab : MonoBehaviour
    {
        [Serializable]
        public class ClickedEvent : UnityEvent<Tab> { }

        [SerializeField]
        private Image _image;

        [SerializeField]
        private Image _selectedImage;

        [SerializeField]
        private Text _text;

        [SerializeField]
        private InputField _inputField;

        public ClickedEvent Clicked;

        public Image Image { get => _image; }

        public string Name
        {
            get => _text.text;
            set
            {
                var tabs = transform.parent.GetComponentsInChildren<Tab>();

                if (tabs.All(t => t.Name != value))
                    _text.text = value;
            }
        }

        public bool Selected
        {
            get => _selectedImage.gameObject.activeSelf;
            set => _selectedImage.gameObject.SetActive(value);
        }

        public void Select(Color color)
        {
            Selected = true;
            _image.color = color;
        }

        public void Deselect(Color color)
        {
            Selected = false;
            _image.color = color;
        }

        public void Edit()
        {
            _inputField.gameObject.SetActive(true);
            _inputField.text = Name;
            _inputField.Select();
        }

        #region Event handlers
        public void Button_OnClick() => Clicked.Invoke(this);

        public void InputField_OnEndEdit(string value)
        {
            _inputField.gameObject.SetActive(false);

            Name = value;
        }
        #endregion
    }
}