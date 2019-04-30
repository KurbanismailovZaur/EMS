using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Linq;

namespace UI.Referencing
{
    public class InputController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private InputField _input;

        private Text _targetText;
        
        private Func<string, string> _endCheckFunction;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private References _references;

        public InputField Input => _input;

        public void Edit(Cell cell)
        {
            var cellRectTransform = ((RectTransform)cell.transform);

            var position = ((RectTransform)cellRectTransform.parent).anchoredPosition + cellRectTransform.anchoredPosition;
            var sizeDelta = cellRectTransform.sizeDelta;

            _rectTransform.anchoredPosition = position;
            _rectTransform.sizeDelta = sizeDelta;

            SetEditType(cell.CellType);

            Input.text = cell.Text.text;
            _targetText = cell.Text;

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
                    _endCheckFunction = CheckInt;
                    break;
                case Cell.Type.NullableInt:
                    _input.contentType = InputField.ContentType.IntegerNumber;
                    _endCheckFunction = CheckNullableInt;
                    break;
                case Cell.Type.String:
                    _input.contentType = InputField.ContentType.Alphanumeric;
                    _endCheckFunction = CheckString;
                    break;
                case Cell.Type.NullableString:
                    _input.contentType = InputField.ContentType.Alphanumeric;
                    _endCheckFunction = CheckNullableString;
                    break;
                case Cell.Type.Float:
                    _input.contentType = InputField.ContentType.DecimalNumber;
                    _endCheckFunction = CheckFloat;
                    break;
                case Cell.Type.NullableFloat:
                    _input.contentType = InputField.ContentType.DecimalNumber;
                    _endCheckFunction = CheckFloat;
                    break;
                case Cell.Type.Material:
                    _input.contentType = InputField.ContentType.IntegerNumber;
                    _endCheckFunction = CheckMaterial;
                    break;
                case Cell.Type.NullableMaterial:
                    _input.contentType = InputField.ContentType.IntegerNumber;
                    _endCheckFunction = CheckNullableMaterial;
                    break;
            }
        }

        #region End checkers
        private string CheckInt(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? "0" : text;
        }

        private string CheckNullableInt(string text) => text;

        private string CheckString(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? _targetText.text : text;
        }

        private string CheckNullableString(string text) => text;

        private string CheckFloat(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? _targetText.text : text;
        }

        private string CheckNullableFloat(string text) => text;

        private string CheckMaterial(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return _targetText.text;

            var materials = (Materials)_references.GetTable("Materials");
            var materialExist = materials.Codes.GetChildren().Select(ch => ch.GetComponent<Cell>().Text).Where(t => t.text == text).Count() > 0;

            return materialExist ? text : _targetText.text;
        }

        private string CheckNullableMaterial(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var materials = (Materials)_references.GetTable("Materials");
            var children = materials.Codes.GetChildren();
            var selected = children.Select(ch => ch.GetComponent<Cell>().Text);
            var materialExist = selected.First(t => t.text == text) != null;

            return materialExist ? text : _targetText.text;
        }
        #endregion

        #region Event handlers
        public void InputField_OnEndEdit(string text)
        {
            _targetText.text = _endCheckFunction(text);

            RoutineHelper.Instance.StartCoroutine(nameof(HoldScrollsRoutine), HoldScrollsRoutine());

            gameObject.SetActive(false);
        }
        #endregion
    }
}