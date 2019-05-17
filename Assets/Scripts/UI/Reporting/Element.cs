using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace UI.Reporting
{
	public class Element : MonoBehaviour 
	{
        public static class Factory
        {
            public static Element Create(Element prefab, string name, Transform _container)
            {
                var element = Instantiate(prefab);
                element.Name = name;
                element.transform.SetParent(_container);

                return element;
            }
        }

        [Serializable]
        public class ClickedEvent : UnityEvent<Element> { }

        [SerializeField]
        private Text _nameText;

        public ClickedEvent Clicked;

        public string Name { get => _nameText.text; set => _nameText.text = value; }

        public void Button_OnClick() => Clicked.Invoke(this);
    }
}