using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using System;

namespace UI.Referencing
{
    public class InputController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private Transform _content;

        [SerializeField]
        private InputField _input;

        private Text _targetText;

        private Func<string, string> _endCheckFunction;

        public InputField Input => _input;

        public void Edit(Cell cell)
        {
            transform.SetParent(cell.transform);
            _rectTransform.Stretch();

            Input.text = cell.Text.text;
            _targetText = cell.Text;

            SetEditType(cell.CellType);

            gameObject.SetActive(true);

            Input.Select();
        }

        private void SetEditType(Cell.Type cellType)
        {
            switch (cellType)
            {
                case Cell.Type.Int:
                    _input.contentType = InputField.ContentType.IntegerNumber;
                    _endCheckFunction = CheckInt;
                    break;
                case Cell.Type.String:
                    _input.contentType = InputField.ContentType.Alphanumeric;
                    _endCheckFunction = CheckString;
                    break;
                case Cell.Type.NullableString:
                    _input.contentType = InputField.ContentType.Alphanumeric;
                    _endCheckFunction = CheckNullableString;
                    break;
            }
        }

        #region End checkers
        private string CheckInt(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? "0" : text;
        }

        private string CheckString(string text)
        {
            return text;
        }

        private string CheckNullableString(string text)
        {
            return text;
        }
        #endregion

        #region Event handlers
        public void InputField_OnEndEdit(string text)
        {
            _targetText.text = CheckInt(text);

            transform.SetParent(_content);
            gameObject.SetActive(false);
        }
        #endregion
    }
}