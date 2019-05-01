using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Linq;
using System.Globalization;

namespace UI.Referencing
{
    public class InputController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private InputField _input;

        private Cell _targetCell;

        private Action<string> _setFunction;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private References _references;

        private static readonly NumberFormatInfo _numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };

        public InputField Input => _input;

        public void Edit(Cell cell)
        {
            var position = ((RectTransform)cell.RectTransform.parent).anchoredPosition + cell.RectTransform.anchoredPosition;
            var sizeDelta = cell.RectTransform.sizeDelta;

            _rectTransform.anchoredPosition = position;
            _rectTransform.sizeDelta = sizeDelta;

            SetEditType(cell.CellType);

            Input.text = cell.NullableStringValue;
            _targetCell = cell;

            RoutineHelper.Instance.StartCoroutine(nameof(HoldScrollsRoutine), HoldScrollsRoutine());

            gameObject.SetActive(true);

            Input.Select();
        }

        private IEnumerator HoldScrollsRoutine()
        {
            var positions = _scrollRect.normalizedPosition;

            yield return null;

            _scrollRect.normalizedPosition = positions;
        }

        private void SetEditType(Cell.Type cellType)
        {
            switch (cellType)
            {
                case Cell.Type.Int:
                    _input.contentType = InputField.ContentType.IntegerNumber;
                    _setFunction = SetInt;
                    break;
                case Cell.Type.NullableInt:
                    _input.contentType = InputField.ContentType.IntegerNumber;
                    _setFunction = SetNullableInt;
                    break;
                case Cell.Type.String:
                    _input.contentType = InputField.ContentType.Standard;
                    _setFunction = SetString;
                    break;
                case Cell.Type.NullableString:
                    _input.contentType = InputField.ContentType.Standard;
                    _setFunction = SetNullableString;
                    break;
                case Cell.Type.Float:
                    _input.contentType = InputField.ContentType.DecimalNumber;
                    _setFunction = SetFloat;
                    break;
                case Cell.Type.NullableFloat:
                    _input.contentType = InputField.ContentType.DecimalNumber;
                    _setFunction = SetNullableFloat;
                    break;
            }
        }

        #region Set functions
        private void SetInt(string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && int.TryParse(text, out int value))
                _targetCell.IntValue = value;
        }

        private void SetNullableInt(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _targetCell.NullableIntValue = null;
                return;
            }

            if (int.TryParse(text, out int value))
                _targetCell.NullableIntValue = value;
        }

        private void SetString(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                _targetCell.StringValue = text;
        }

        private void SetNullableString(string text) => _targetCell.StringValue = text;

        private string CheckNullableInt(string text) => text;

        private void SetFloat(string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && float.TryParse(text, NumberStyles.Float, _numberFormatInfo, out float value))
                _targetCell.FloatValue = value;
        }

        private void SetNullableFloat(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _targetCell.NullableFloatValue = null;
                return;
            }

            if (float.TryParse(text, NumberStyles.Float, _numberFormatInfo, out float value))
                _targetCell.NullableFloatValue = value;
        }
        #endregion

        #region Event handlers
        public void InputField_OnEndEdit(string text)
        {
            _setFunction(text);

            RoutineHelper.Instance.StartCoroutine(nameof(HoldScrollsRoutine), HoldScrollsRoutine());

            gameObject.SetActive(false);
        }
        #endregion
    }
}