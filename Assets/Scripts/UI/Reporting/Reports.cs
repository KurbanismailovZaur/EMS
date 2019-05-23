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
using Management;
using Management.Interop;
using UI.Exploring.FileSystem;

namespace UI.Reporting
{
	public class Reports : MonoBehaviour 
	{
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
        private Button _cancelButton;

        private bool _isOpen;

        private void Start()
        {
            _generateButton.onClick.AddListener(GenerateButton_OnClick);

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

        private void Generate() => StartCoroutine(GenerateRoutine());

        private IEnumerator GenerateRoutine()
        {
            yield return FileExplorer.Instance.SaveFile("Сгенерировать Отчеты", null, "xlsx", "reports.xlsx");

            if (FileExplorer.Instance.LastResult == null) yield break;

            var points = _kvid6.SelectedElements.Select(el => el.Name).ToArray();
            var wires = _kvid3.SelectedElements.Select(el => el.Name).ToArray();

            DatabaseManager.Instance.RemoveSelectPointAndWire();
            DatabaseManager.Instance.UpdateSelectPointAndWire(points, wires);

            PythonManager.Instance.GenerateReports(FileExplorer.Instance.LastResult);

            Close();
        }

        #region Event handlers
        private void GenerateButton_OnClick() => Generate();

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
        #endregion
    }
}