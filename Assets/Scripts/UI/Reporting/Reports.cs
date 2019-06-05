using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Debug;
using System;
using System.Linq;
using Exceptions;
using Management;
using Management.Calculations;
using Management.Interop;
using Management.Wires;
using UI.Dialogs;
using UI.Exploring.FileSystem;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Reporting
{
    public class Reports : MonoBehaviour
    {
        private enum GenerateType
        {
            Points,
            Wires,
            All,
            Selected
        }

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private SelectionManager _kvid6;

        [SerializeField]
        private SelectionManager _kvid3;

        [SerializeField]
        private GameObject _kvid6Blocker;

        [SerializeField]
        private GameObject _kvid3Blocker;

        [SerializeField]
        private Button _generateButton;

        [SerializeField]
        private Button _generateCloseButton;

        [SerializeField]
        private Button _generatePointsButton;

        [SerializeField]
        private Button _generateWiresButton;

        [SerializeField]
        private Button _generateAllButton;

        [SerializeField]
        private Button _generateSelectedButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private Button _generatePanelButton;

        private bool _isOpen;

        private void Start()
        {
            _generateButton.onClick.AddListener(GenerateButton_OnClick);

            _generatePanelButton.onClick.AddListener(GeneratePanelButton_OnClick);

            _generateCloseButton.onClick.AddListener(GenerateCloseButton_OnClick);

            _generatePointsButton.onClick.AddListener(GeneratePointsButton_OnClick);

            _generateWiresButton.onClick.AddListener(GenerateWiresButton_OnClick);

            _generateAllButton.onClick.AddListener(GenerateAllButton_OnClick);

            _generateSelectedButton.onClick.AddListener(GenerateSelectedButton_OnClick);

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
            var wires = CalculationsManager.Instance.MutualActionOfBCSAndBA.WiresNames;

            _kvid6.Initialize(points);
            _kvid3.Initialize(wires);

            _generatePointsButton.interactable = CalculationsManager.Instance.ElectricFieldStrenght.Points.Count > 0;
            _generateWiresButton.interactable = CalculationsManager.Instance.MutualActionOfBCSAndBA.WiresNames?.Length > 0;

            _generateAllButton.interactable = _generatePointsButton.interactable && _generateWiresButton.interactable;
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

        private IEnumerator GenerateRoutine(GenerateType generateType)
        {
            _generatePanelButton.gameObject.SetActive(false);

            yield return FileExplorer.Instance.SaveFile("Сгенерировать Отчеты", null, "xlsx", "reports.xlsx");

            if (FileExplorer.Instance.LastResult == null) yield break;

            string[] points = { };
            string[] wires = { };

            switch (generateType)
            {
                case GenerateType.Points:
                    points = CalculationsManager.Instance.ElectricFieldStrenght.Points.Select(p => p.Code).ToArray();
                    break;
                case GenerateType.Wires:
                    wires = WiringManager.Instance.Wiring.Wires.Select(w => w.Name).ToArray();
                    break;
                case GenerateType.All:
                    points = CalculationsManager.Instance.ElectricFieldStrenght.Points.Select(p => p.Code).ToArray();
                    wires = WiringManager.Instance.Wiring.Wires.Select(w => w.Name).ToArray();
                    break;
                case GenerateType.Selected:
                    points = _kvid6.SelectedElements.Select(el => el.Name).ToArray();
                    wires = _kvid3.SelectedElements.Select(el => el.Name).ToArray();
                    break;
            }

            DatabaseManager.Instance.RemoveSelectPointAndWire();
            DatabaseManager.Instance.UpdateSelectPointAndWire(points, wires);

            try
            {
                PythonManager.Instance.GenerateReports(FileExplorer.Instance.LastResult);
            }
            catch (Exception ex)
            {
                ErrorDialog.Instance.ShowError("Не удалось сгенерировать отчеты", ex);
            }

            Close();
        }

        #region Event handlers
        private void GenerateButton_OnClick() => _generatePanelButton.gameObject.SetActive(true);

        private void GeneratePanelButton_OnClick() => _generatePanelButton.gameObject.SetActive(false);

        private void GenerateCloseButton_OnClick() => _generatePanelButton.gameObject.SetActive(false);

        private void GeneratePointsButton_OnClick() => StartCoroutine(GenerateRoutine(GenerateType.Points));

        private void GenerateWiresButton_OnClick() => StartCoroutine(GenerateRoutine(GenerateType.Wires));

        private void GenerateAllButton_OnClick() => StartCoroutine(GenerateRoutine(GenerateType.All));

        private void GenerateSelectedButton_OnClick() => StartCoroutine(GenerateRoutine(GenerateType.Selected));

        private void CancelButton_OnClick() => Close();

        public void ElectricFieldCalculated()
        {
            _kvid6Blocker.SetActive(false);
        }

        public void ElectricFieldRemoved()
        {
            _kvid6Blocker.SetActive(true);
        }

        public void BKSandBACalculated()
        {
            _kvid3Blocker.SetActive(false);
        }

        public void BKSandBARemoved()
        {
            _kvid3Blocker.SetActive(true);
        }

        public void SelectionManager_Changed()
        {
            _generateSelectedButton.interactable = (_kvid6.SelectedElements.Length + _kvid3.SelectedElements.Length > 0) ? true : false;
        }
        #endregion
    }
}