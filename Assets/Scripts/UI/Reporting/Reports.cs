using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Exceptions;
using UnityEngine.UI;
using Management.Calculations;
using System.Linq;
using Management.Wires;
using System;
using UnityEngine.Events;

namespace UI.Reporting
{
	public class Reports : MonoBehaviour 
	{
        public enum GenerateType
        {
            Points,
            Wires,
            All
        }
        
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private SelectionManager _kvid6;

        [SerializeField]
        private SelectionManager _kvid3;

        [SerializeField]
        private Button _generateButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private Button _generateOptions;

        [SerializeField]
        private Button _generatePointsButton;

        [SerializeField]
        private Button _generateWiresButton;

        [SerializeField]
        private Button _GenerateAllButton;

        private bool _isOpen;

        private void Start()
        {
            _generateButton.onClick.AddListener(GenerateButton_OnClick);
            _generateOptions.onClick.AddListener(GenerateOptionsButton_OnClick);
            _generatePointsButton.onClick.AddListener(GeneratePointsButton_OnClick);
            _generateWiresButton.onClick.AddListener(GenerateWiresButton_OnClick);
            _GenerateAllButton.onClick.AddListener(GenerateAllButton_OnClick);

            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public void Open()
        {
            if (_isOpen) throw new BusyException("Already opened");

            _isOpen = true;

            Initialize();

            Show();
        }

        private void Initialize()
        {
            var points = CalculationsManager.Instance.ElectricFieldStrenght.Points.Select(p => p.Code).ToArray();
            var wires = WiringManager.Instance.Wiring?.Wires.Select(w => w.Name).ToArray() ?? new string[0];

            _kvid6.Initialize(points);
            _kvid3.Initialize(wires);
        }

        private void Close()
        {
            Hide();

            _isOpen = false;
        }

        private void Show() => SetVisibility(1f, true);

        private void Hide() => SetVisibility(0f, false);

        private void SetVisibility(float alpha, bool blockRaycast)
        {
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = blockRaycast;
        }

        private void ShowGerateOptions() => _generateOptions.gameObject.SetActive(true);

        private void HideGerateOptions() => _generateOptions.gameObject.SetActive(false);

        private void SelectAndClose(GenerateType type)
        {
            Close();
        }

        #region Event handlers
        private void GenerateButton_OnClick() => ShowGerateOptions();

        private void GenerateOptionsButton_OnClick() => HideGerateOptions();

        private void GeneratePointsButton_OnClick() => SelectAndClose(GenerateType.Points);

        private void GenerateWiresButton_OnClick() => SelectAndClose(GenerateType.Wires);

        private void GenerateAllButton_OnClick() => SelectAndClose(GenerateType.All);

        private void CancelButton_OnClick() => Close();
        #endregion
    }
}