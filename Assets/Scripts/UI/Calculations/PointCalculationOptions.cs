using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Exceptions;
using UnityEngine.UI;
using System;

namespace UI.Calculations
{
	public class PointCalculationOptions : MonoBehaviour 
	{
        public enum CalculationType
        {
            Default,
            Import
        }

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private InputField _pointsCountInput;

        [SerializeField]
        private Button _importButton;

        [SerializeField]
        private Button _submitButton;

        [SerializeField]
        private Button _cancelButton;

        private bool _isOpen;

        public int PointsByAxis { get; private set; } = 5;

        public Coroutine Routine { get; private set; }

        public CalculationType? LastResultType { get; private set; }

        private void Start()
        {
            _pointsCountInput.onEndEdit.AddListener(PointsCountInput_OnEndEdit);

            _importButton.onClick.AddListener(ImportButton_OnClick);
            _submitButton.onClick.AddListener(SubmitButton_OnClick);
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        public Coroutine Open()
        {
            if (_isOpen) throw new BusyException("Already opened");

            _isOpen = true;

            Show();

            return Routine = StartCoroutine(BusyRoutine());
        }

        private IEnumerator BusyRoutine()
        {
            while (_isOpen) yield return null;
            Routine = null;
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

        private void SaveDecision(CalculationType? type)
        {
            LastResultType = type;
            Close();
        }

        #region Event handlers
        private void PointsCountInput_OnEndEdit(string text)
        {           
            var value = text == string.Empty || !int.TryParse(text, out int parsedValue)? 125 : Mathf.Clamp(parsedValue, 125, 103823);
            PointsByAxis = (int)Mathf.Round(Mathf.Pow(value, 1f / 3f));

            _pointsCountInput.text = ((int)Mathf.Pow(PointsByAxis, 3f)).ToString();
        }

        private void ImportButton_OnClick() => SaveDecision(CalculationType.Import);

        private void SubmitButton_OnClick() => SaveDecision(CalculationType.Default);

        private void CancelButton_OnClick() => SaveDecision(null);
        #endregion
    }
}