using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Globalization;

namespace UI.Referencing
{
    public class Cell : MonoBehaviour
    {
        public enum Type
        {
            Int,
            NullableInt,
            UniqueInt,
            String,
            NullableString,
            UniqueString,
            Float,
            NullableFloat
        }

        public static class Factory
        {
            private static Cell Create(Cell cellPrefab, Type type, Action<Cell> valueSetter, Column column, Action<Cell> cellClickHandler)
            {
                var cell = Instantiate(cellPrefab, column.transform);
                cell._type = type;
                cell.DoubleClicked += cellClickHandler;
                valueSetter(cell);
                cell.Column = column;

                return cell;
            }

            public static Cell Create(Cell cellPrefab, int value, Column Column, Action<Cell> cellClickHandler)
            {
                return Create(cellPrefab, Type.Int, (c) => c.IntValue = value, Column, cellClickHandler);
            }

            public static Cell Create(Cell cellPrefab, int? value, Column column, Action<Cell> cellClickHandler)
            {
                return Create(cellPrefab, Type.NullableInt, (c) => c.NullableIntValue = value, column, cellClickHandler);
            }

            public static Cell Create(Cell cellPrefab, string value, bool isNullableString, Column column, Action<Cell> cellClickHandler)
            {
                return Create(cellPrefab, isNullableString ? Type.NullableString : Type.String, (c) => { if (isNullableString) c.NullableStringValue = value; else c.StringValue = value; }, column, cellClickHandler);
            }

            public static Cell Create(Cell cellPrefab, float value, Column column, Action<Cell> cellClickHandler)
            {
                return Create(cellPrefab, Type.Float, (c) => c.FloatValue = value, column, cellClickHandler);
            }

            public static Cell Create(Cell cellPrefab, float? value, Column column, Action<Cell> cellClickHandler)
            {
                return Create(cellPrefab, Type.NullableFloat, (c) => c.NullableFloatValue = value, column, cellClickHandler);
            }

            public static Cell CreateUnique(Cell cellPrefab, int value, Column column, Action<Cell> cellClickHandler)
            {
                return Create(cellPrefab, Type.UniqueInt, (c) => c.IntValue = value, column, cellClickHandler);
            }

            public static Cell CreateUnique(Cell cellPrefab, string value, Column column, Action<Cell> cellClickHandler)
            {
                return Create(cellPrefab, Type.UniqueString, (c) => c.StringValue = value, column, cellClickHandler);
            }
        }

        [Serializable]
        public class DoubleClickedEvent : UnityEvent<Cell> { }

        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private Text _text;

        [SerializeField]
        private Type _type;

        private static readonly NumberFormatInfo _numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
        
        public event Action<Cell> DoubleClicked;

        public RectTransform RectTransform => _rectTransform;

        private Coroutine _doubleClickRoutine;

        public event Action<Cell> Changed;

        public Column Column { get; private set; }

        public Type CellType => _type;

        public int IntValue
        {
            get => int.Parse(_text.text);
            set => SetText(value.ToString());
        }

        public int? NullableIntValue
        {
            get => string.IsNullOrWhiteSpace(_text.text) ? null : (int?)IntValue;
            set => SetText(value.ToString());
        }

        public string StringValue
        {
            get => string.IsNullOrWhiteSpace(_text.text) ? throw new FormatException("Text can not be a null or white space") : _text.text;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new FormatException("Value can not be a null or white space");

                SetText(value);
            }
        }

        public string NullableStringValue
        {
            get => _text.text;
            set => SetText(value);
        }

        public float FloatValue
        {
            get => float.Parse(_text.text, _numberFormatInfo);
            set => SetText(FormatFloat(value));
        }

        public float? NullableFloatValue
        {
            get => string.IsNullOrWhiteSpace(_text.text) ? null : (float?)FloatValue;
            set => SetText(value == null ? null : FormatFloat((float)value));
        }

        private void SetText(string value)
        {
            value = value?.Trim();

            if (_text.text == value) return;
            
            _text.text = value;
            Changed?.Invoke(this);
        }

        private string FormatFloat(float value) => value.ToString("F32", _numberFormatInfo).TrimEnd('0').TrimEnd('.');

        private void HandleClick()
        {
            if (_doubleClickRoutine != null)
            {
                StopCoroutine(_doubleClickRoutine);
                _doubleClickRoutine = null;

                DoubleClicked?.Invoke(this);
            }
            else
                _doubleClickRoutine = StartCoroutine(DoubleClickRoutine());
        }

        private IEnumerator DoubleClickRoutine()
        {
            yield return new WaitForSeconds(0.250f);
            _doubleClickRoutine = null;
        }

        #region Event handlers
        public void Button_OnClick() => HandleClick();
        #endregion
    }
}