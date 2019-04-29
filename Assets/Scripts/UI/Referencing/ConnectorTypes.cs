using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Management.Referencing;
using System.Linq;

namespace UI.Referencing
{
    public class ConnectorTypes : Table
    {
        [SerializeField]
        private RectTransform _codes;

        [SerializeField]
        private RectTransform _types;

        public override void LoadData(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            ClearData();

            foreach (var mark in ReferenceManager.Instance.ConnectorTypes)
                Add(cellPrefab, mark.Code.ToString(), mark.Type, cellClickHandler);
        }

        public override void Add(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            var code = $"Р{_codes.GetChildren().Select(ch => ch.GetComponent<Cell>().Text).Max(t => int.Parse(t.text.Substring(1))) + 1}";

            Add(cellPrefab, code, "тип", cellClickHandler);
        }

        private void Add(Cell cellPrefab, string code, string type, Action<Cell> cellClickHandler)
        {
            Cell.Factory.Create(cellPrefab, Cell.Type.String, _codes, code, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.String, _types, type, cellClickHandler);
        }
    }
}