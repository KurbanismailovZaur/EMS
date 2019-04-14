﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Browsing.FileSystem
{
    public class Element : MonoBehaviour, IPointerDownHandler
    {
        #region Classes
        public static class Factory
        {
            public static Element Create(Element prefab, Transform container, string path, string name)
            {
                Element file = Instantiate(prefab, container);
                file.Path = path;
                file.Name = name;

                return file;
            }
        }

        [Serializable]
        public class ClickedEvent : UnityEvent<Element> { }

        [Serializable]
        public class DoubleClickedEvent : UnityEvent<Element> { }
        #endregion

        [SerializeField]
        private Text _text;

        [SerializeField]
        private Image _image;

        private string _name;

        public ClickedEvent Clicked;

        public DoubleClickedEvent DoubleClicked;

        public string Path { get; private set; }

        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
                _text.text = _name;
            }
        }

        public Color Color
        {
            get => _image.color;
            set => _image.color = value;
        }

        private Coroutine _clickDetectRoutine;

        private static float _doubleClickDelay = 0.25f;

        private void ClickDetect()
        {
            if (_clickDetectRoutine == null)
            {
                _clickDetectRoutine = StartCoroutine(ClickDetectRoutine());

                Clicked.Invoke(this);
            }
            else
            {
                StopCoroutine(_clickDetectRoutine);
                _clickDetectRoutine = null;

                DoubleClicked.Invoke(this);
            }

        }

        private IEnumerator ClickDetectRoutine()
        {
            yield return new WaitForSeconds(_doubleClickDelay);

            _clickDetectRoutine = null;
        }

        public void OnPointerDown(PointerEventData eventData) => ClickDetect();
    }
}