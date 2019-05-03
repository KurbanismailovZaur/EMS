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
using UI.Tables.Concrete;
using UI.TableViews;

namespace UI.Tables
{
    public class InputController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private InputField _input;

        private Cell _targetCell;

        private Action<string> _setFunction;
        
        private ScrollRect _scrollRect;

        [SerializeField]
        private ReferencesView _references;

        private static readonly NumberFormatInfo _numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };

        public InputField Input => _input;

        public void Edit(Cell cell)
        {
            _scrollRect = cell.GetComponentInParent<ScrollRect>();

            _targetCell = cell;
            transform.SetParent(_targetCell.GetComponentInParent<ScrollRect>().content);

            var position = ((RectTransform)_targetCell.RectTransform.parent).anchoredPosition + _targetCell.RectTransform.anchoredPosition;
            var sizeDelta = _targetCell.RectTransform.sizeDelta;

            _rectTransform.anchoredPosition = position;
            _rectTransform.sizeDelta = sizeDelta;

            SetEditType(_targetCell.CellType);

            Input.text = _targetCell.NullableStringValue;

            RoutineHelper.Instance.StartCoroutine(nameof(HoldScrollsRoutine), HoldScrollsRoutine());
            gameObject.SetActive(true);

            Input.Select();
        }

        private IEnumerator HoldScrollsRoutine()
        {
            var position = _scrollRect.normalizedPosition;

            while (true)
            {
                _scrollRect.normalizedPosition = position;
                yield return null;
            }
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
                case Cell.Type.UniqueInt:
                    _input.contentType = InputField.ContentType.IntegerNumber;
                    _setFunction = SetUniqueInt;
                    break;
                case Cell.Type.String:
                    _input.contentType = InputField.ContentType.Standard;
                    _setFunction = SetString;
                    break;
                case Cell.Type.NullableString:
                    _input.contentType = InputField.ContentType.Standard;
                    _setFunction = SetNullableString;
                    break;
                case Cell.Type.UniqueString:
                    _input.contentType = InputField.ContentType.Standard;
                    _setFunction = SetUniqueString;
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

        private void SetUniqueInt(string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && int.TryParse(text, out int value) && !_targetCell.Column.Cells.Any(c => c.IntValue == value))
                _targetCell.IntValue = value;
        }

        private void SetString(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                _targetCell.StringValue = text;
        }

        private void SetNullableString(string text) => _targetCell.StringValue = text;

        private void SetUniqueString(string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && !_targetCell.Column.Cells.Any(c => c.StringValue == text))
                _targetCell.StringValue = text;
        }

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

        private IEnumerator StopHoldScrollsRoutine(Vector2 position)
        {
            yield return null;

            while (_scrollRect.normalizedPosition != position) yield return null;

            RoutineHelper.Instance.StopCoroutine(nameof(HoldScrollsRoutine));
        }

        #region Event handlers
        public void InputField_OnEndEdit(string text)
        {
            _setFunction(text);

            RoutineHelper.Instance.StartCoroutine(nameof(StopHoldScrollsRoutine), StopHoldScrollsRoutine(_scrollRect.normalizedPosition));

            transform.SetParent(GetComponentInParent<Canvas>().transform);
            gameObject.SetActive(false);
        }
        #endregion
    }
}