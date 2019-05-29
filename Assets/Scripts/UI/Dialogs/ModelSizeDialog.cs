using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using Management;

namespace UI.Dialogs
{
	public class ModelSizeDialog : Dialog<ModelSizeDialog> 
	{
        public enum DecisionType
        {
            Cancel,
            Apply
        }

        [SerializeField]
        private InputField _xInput;

        [SerializeField]
        private InputField _yInput;

        [SerializeField]
        private InputField _zInput;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private Button _applyButton;

        public DecisionType Decision { get; private set; }

        public Vector3 Size => new Vector3(float.Parse(_xInput.text), float.Parse(_yInput.text), float.Parse(_zInput.text));

        private void Start()
        {
            _cancelButton.onClick.AddListener(CancelButton_OnClick);
            _applyButton.onClick.AddListener(ApplyButton_OnClick);

            _xInput.onEndEdit.AddListener(XInputField_OnEndEdit);
            _yInput.onEndEdit.AddListener(YInputField_OnEndEdit);
            _zInput.onEndEdit.AddListener(ZInputField_OnEndEdit);
        }

        public Coroutine Open()
        {
            var size = DatabaseManager.Instance.GetModelSize();

            _xInput.text = size.x.ToString();
            _yInput.text = size.y.ToString();
            _zInput.text = size.z.ToString();

            Show();

            return StartCoroutine(OpenRoutine());
        }

        private IEnumerator OpenRoutine()
        {
            while (IsShowed) yield return null;
        }

        private void CloseWith(DecisionType decision)
        {
            Decision = decision;
            Hide();
        }

        #region Event handlers
        private void CancelButton_OnClick() => CloseWith(DecisionType.Cancel);

        private void ApplyButton_OnClick() => CloseWith(DecisionType.Apply);

        private void XInputField_OnEndEdit(string value) => _xInput.text = string.IsNullOrWhiteSpace(value) ? "0" : value.StartsWith(".") ? $"0{value}" : value;

        private void YInputField_OnEndEdit(string value) => _yInput.text = string.IsNullOrWhiteSpace(value) ? "0" : value.StartsWith(".") ? $"0{value}" : value;

        private void ZInputField_OnEndEdit(string value) => _zInput.text = string.IsNullOrWhiteSpace(value) ? "0" : value.StartsWith(".") ? $"0{value}" : value;
        #endregion
    }
}