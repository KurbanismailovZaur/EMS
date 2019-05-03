using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityObject = UnityEngine.Object;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using Management.Wires;

namespace UI.Tables.Concrete.KVIDS
{
    public class KVID6Table : Table
    {
        public class KVID6Panel : Panel
        {
            public RemoveButton RemoveButton { get; private set; }

            public Cell Code { get; private set; }

            public Cell X { get; private set; }

            public Cell Y { get; private set; }

            public Cell Z { get; private set; }

            public KVID6Panel(RemoveButton removeButton, Cell code, Cell x, Cell y, Cell z)
            {
                RemoveButton = removeButton;
                Code = code;
                X = x;
                Y = y;
                Z = z;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(RemoveButton.gameObject);
                UnityObject.Destroy(Code.gameObject);
                UnityObject.Destroy(X.gameObject);
                UnityObject.Destroy(Y.gameObject);
                UnityObject.Destroy(Z.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "Code":
                        return Code;
                    case "X":
                        return X;
                    case "Y":
                        return Y;
                    case "Z":
                        return Z;
                    default:
                        throw new ArgumentException($"No cell with name \"{ name }\"");
                }
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                throw new ArgumentException($"No reference cell with name \"{ name }\"");
            }
        }

        [SerializeField]
        private RemoveButton _removeButtonPrefab;

        #region Columns
        [Header("Columns")]
        [SerializeField]
        private Column _removes;

        [SerializeField]
        private Column _codes;

        [SerializeField]
        private Column _xs;

        [SerializeField]
        private Column _ys;

        [SerializeField]
        private Column _zs;
        #endregion

        private List<KVID6Panel> _kvid6Panels = new List<KVID6Panel>();

        private int _nextCodeIndex;

        #region Columns
        public Column Removes => _removes;

        public Column Codes => _xs;

        public Column Xs => _xs;

        public Column Ys => _ys;

        public Column Zs => _zs;
        #endregion

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add(GetNextCode(), 0f, 0f, 0f, cellClickHandler);

        private KVID6Panel Add(string code, float x, float y, float z, Action<Cell> cellClickHandler)
        {
            var removeButton = RemoveButton.Factory.Create(_removeButtonPrefab, _removes.transform, RemoveButton_Clicked);
            var codeCell = Cell.Factory.CreateUnique(_cellPrefab, code, _codes, cellClickHandler);
            var xCell = Cell.Factory.Create(_cellPrefab, x, _xs, cellClickHandler);
            var yCell = Cell.Factory.Create(_cellPrefab, y, _ys, cellClickHandler);
            var zCell = Cell.Factory.Create(_cellPrefab, z, _zs, cellClickHandler);

            var panel = new KVID6Panel(removeButton, codeCell, xCell, yCell, zCell);
            _kvid6Panels.Add(panel);

            removeButton.Panel = panel;
            AddPanelToColumns(panel);

            Added.Invoke(panel);

            return panel;
        }

        private string GetNextCode()
        {
            while (_kvid6Panels.Find(p => p.Code.StringValue == $"Т{_nextCodeIndex}") != null) _nextCodeIndex += 1;

            return $"Т{_nextCodeIndex}";
        }

        public override void Clear()
        {
            foreach (var panel in _kvid6Panels)
                panel.Destroy();

            _kvid6Panels.Clear();
        }

        public override void Remove(Panel panel)
        {
            if (!_kvid6Panels.Contains((KVID6Panel)panel)) return;

            _kvid6Panels.Remove((KVID6Panel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();

            Removed.Invoke(panel);
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvid3Panel = (KVID6Panel)panel;

            _codes.AddCell(kvid3Panel.Code);
            _xs.AddCell(kvid3Panel.X);
            _ys.AddCell(kvid3Panel.Y);
            _zs.AddCell(kvid3Panel.Z);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var kvid3Panel = (KVID6Panel)panel;

            _codes.RemoveCell(kvid3Panel.Code);
            _xs.RemoveCell(kvid3Panel.X);
            _ys.RemoveCell(kvid3Panel.Y);
            _zs.RemoveCell(kvid3Panel.Z);
        }

        public List<(string code, Vector3 position)> GetPoints()
        {
            var points = new List<(string code, Vector3 position)>();

            foreach (var panel in _kvid6Panels)
                points.Add((panel.Code.StringValue, new Vector3(panel.X.FloatValue, panel.Y.FloatValue, panel.Z.FloatValue)));

            return points;
        }

        #region Event handlers
        private void RemoveButton_Clicked(RemoveButton removeButton, Panel panel)
        {
            Remove(panel);
            Destroy(removeButton.gameObject);
        }
        #endregion
    }
}