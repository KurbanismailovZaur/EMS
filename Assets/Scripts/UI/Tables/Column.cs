using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;
using System.Collections.ObjectModel;
using System.Linq;

namespace UI.Tables
{
	public class Column : MonoBehaviour 
	{
        [Serializable]
        public class RectTransformChangedEvent : UnityEvent<Vector2> { }

        public RectTransformChangedEvent RectTransformChanged;

        public List<object> _cells = new List<object>();

        public void AddCell(Cell cell) => _cells.Add(cell);

        public void AddCell(ReferenceCell cell) => _cells.Add(cell);

        public void RemoveCell(Cell cell) => _cells.Remove(cell);

        public void RemoveCell(ReferenceCell cell) => _cells.Remove(cell);

        public ReadOnlyCollection<Cell> Cells => new ReadOnlyCollection<Cell>(_cells.Cast<Cell>().ToList());

        public ReadOnlyCollection<ReferenceCell> ReferenceCells => new ReadOnlyCollection<ReferenceCell>(_cells.Cast<ReferenceCell>().ToList());

        private void OnRectTransformDimensionsChange() => RectTransformChanged.Invoke(((RectTransform)transform).sizeDelta);
    }
}