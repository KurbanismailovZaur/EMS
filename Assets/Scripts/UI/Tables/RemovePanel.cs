using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UI.Tables.Concrete;

namespace UI.Tables
{
	public class RemovePanel : MonoBehaviour 
	{
        public static class Factory
        {
            public static RemovePanel Create(RemovePanel prefab, Transform parent, string label, Table table, Panel panel)
            {
                var removePanel = Instantiate(prefab, parent);
                removePanel._labelText.text = label;
                removePanel._table = table;
                removePanel._panel = panel;

                return removePanel;
            }
        }

        [SerializeField]
        private Text _labelText;

        private Table _table;

        private Panel _panel;

        #region Event handlers
        public void Button_OnClick()
        {
            _table.Remove(_panel);
            Destroy(transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject);
            Destroy(gameObject);
        }
        #endregion
    }
}