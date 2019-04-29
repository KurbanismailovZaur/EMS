using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.UI;

namespace UI.Referencing
{
	public class RemoveDialog : MonoBehaviour 
	{
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Text _titleText;

        [SerializeField]
        private Text _labelText;

        [SerializeField]
        private RectTransform _content;

        [Header("Prefabs")]
        [SerializeField]
        private RemovePanel _removePanelPrefab;

        [SerializeField]
        private RectTransform _deviderPrefab;

        public void Open((string titleRemoveName, string labelName, (string label, Action deleteHandler)[] panelsData) data)
        {
            _titleText.text = $"Удаление {data.titleRemoveName}";
            _labelText.text = data.labelName;

            UpdatePanels(data.panelsData);

            Show();
        }

        public void Close() => Hide();

        private void UpdatePanels((string label, Action deleteHandler)[] panelsData)
        {
            ClearPanels();

            foreach (var (label, deleteHandler) in panelsData)
            {
                RemovePanel.Factory.Create(_removePanelPrefab, _content, label, deleteHandler);
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