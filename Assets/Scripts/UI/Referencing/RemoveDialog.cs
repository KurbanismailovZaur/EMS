using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.UI;
using UI.Referencing.Tables;

namespace UI.Referencing
{
	public class RemoveDialog : MonoBehaviour 
	{
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private RectTransform _content;

        [Header("Prefabs")]
        [SerializeField]
        private RemovePanel _removePanelPrefab;

        [SerializeField]
        private RectTransform _deviderPrefab;

        public void Open(Table table, List<Panel> panels)
        {
            UpdatePanels(table, panels);

            Show();
        }

        public void Close() => Hide();

        private void UpdatePanels(Table table, List<Panel> panels)
        {
            ClearPanels();

            foreach (var panel in panels)
            {
                RemovePanel.Factory.Create(_removePanelPrefab, _content, panel.GetCell(table.RemoveCellName).NullableStringValue, table, panel);
                Instantiate(_deviderPrefab, _content);
            }
        }

        private void ClearPanels()
        {
            foreach (Transform child in _content)
                Destroy(child.gameObject);
        }

        private void Show() => SetCanvasParameters(1f, true);

        private void Hide() => SetCanvasParameters(0f, false);

        private void SetCanvasParameters(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }
	}
}