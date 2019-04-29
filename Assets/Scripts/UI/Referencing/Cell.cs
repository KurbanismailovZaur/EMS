using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace UI.Referencing
{
	public class Cell : MonoBehaviour 
	{
        public enum Type
        {
            Int,
            String,
            NullableString,
            Float,
            NullableFloat,
            Material,
            NullableMaterial
        }

        public static class Factory
        {
            public static Cell Create(Cell cellPrefab, Type type, Transform parent, string value, Action<Cell> cellClickHandler)
            {
                var cell = Instantiate(cellPrefab, parent);

                cell._type = type;
                cell.Text.text = value;

                cell.DoubleClicked += cellClickHandler;

                return cell;
            }
        }

        [Serializable]
        public class DoubleClickedEvent : UnityEvent<Cell> { }

        [SerializeField]
        private Text _text;

        [SerializeField]
        private Type _type;
        
        public event Action<Cell> DoubleClicked;

        public Text Text => _text;

        public Type CellType => _type;

        private Coroutine _doubleClickRoutine;

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