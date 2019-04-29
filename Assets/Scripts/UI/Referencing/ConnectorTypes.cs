using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Management.Referencing;

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
            {
                Cell.Factory.Create(cellPrefab, Cell.Type.String, _codes, mark.Code.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.String, _types, mark.Type, cellClickHandler);
            }
        }
    }
}