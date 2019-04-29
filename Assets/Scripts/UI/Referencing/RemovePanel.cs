using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace UI.Referencing
{
	public class RemovePanel : MonoBehaviour 
	{
        public static class Factory
        {
            public static RemovePanel Create(RemovePanel prefab, Transform parent, string label, Action deleteHandler)
            {
                var panel = Instantiate(prefab, parent);
                panel._labelText.text = label;
                panel._deleteHandler = deleteHandler;

                return panel;
            }
        }

        [SerializeField]
        private Text _labelText;

        public Action _deleteHandler;

        #region Event handlers
        public void Button_OnClick()
        {
            _deleteHandler?.Invoke();
            Destroy(transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject);
            Destroy(gameObject);
        }
        #endregion
    }
}