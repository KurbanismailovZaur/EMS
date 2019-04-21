using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.References
{
    public class Tab : MonoBehaviour
    {
        [Serializable]
        public class ClickedEvent : UnityEvent<Tab> { }

        [SerializeField]
        private Image _image;

        [SerializeField]
        private Image _selectedImage;

        public ClickedEvent Clicked;

        public Image Image { get => _image; }

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

        #region Event handlers
        public void Button_OnClick() => Clicked.Invoke(this);
        #endregion
    }
}