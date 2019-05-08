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
    public class KVID2Table : Table
    {
        public class KVID2Panel : Panel
        {
            public RemoveButton RemoveButton { get; private set; }

            public Cell X { get; private set; }

            public Cell Y { get; private set; }

            public Cell Z { get; private set; }

            public KVID2Panel(RemoveButton removeButton, Cell x, Cell y, Cell z)
            {
                RemoveButton = removeButton;
                X = x;
                Y = y;
                Z = z;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(RemoveButton.gameObject);
                UnityObject.Destroy(X.gameObject);
                UnityObject.Destroy(Y.gameObject);
                UnityObject.Destroy(Z.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
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
        private Column _xs;

        [SerializeField]
        private Column _ys;

        [SerializeField]
        private Column _zs;
        #endregion

        private List<KVID2Panel> _kvid2Panels = new List<KVID2Panel>();

        private int _nextCodeIndex;

        #region Columns
        public Column Removes => _removes;

        public Column Xs => _xs;

        public Column Ys => _ys;

        public Column Zs => _zs;
        #endregion

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add(null, null, null, cellClickHandler);

        private KVID2Panel Add(float? x, float? y, float? z, Action<Cell> cellClickHandler)
        {
            var removeButton = RemoveButton.Factory.Create(_removeButtonPrefab, _removes.transform, RemoveButton_Clicked);
            var xCell = Cell.Factory.Create(_cellPrefab, x, _xs, cellClickHandler);
            var yCell = Cell.Factory.Create(_cellPrefab, y, _ys, cellClickHandler);
            var zCell = Cell.Factory.Create(_cellPrefab, z, _zs, cellClickHandler);

            var panel = new KVID2Panel(removeButton, xCell, yCell, zCell);
            _kvid2Panels.Add(panel);

            removeButton.Panel = panel;
            AddPanelToColumns(panel);

            Added.Invoke(panel);

            return panel;
        }

        public override void Clear()
        {
            foreach (var panel in _kvid2Panels)
                panel.Destroy();

            _kvid2Panels.Clear();
        }

        public override void Remove(Panel panel)
        {
            if (!_kvid2Panels.Contains((KVID2Panel)panel)) return;

            _kvid2Panels.Remove((KVID2Panel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();

            Removed.Invoke(panel);
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvid2Panel = (KVID2Panel)panel;

            _xs.AddCell(kvid2Panel.X);
            _ys.AddCell(kvid2Panel.Y);
            _zs.AddCell(kvid2Panel.Z);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var kvid2Panel = (KVID2Panel)panel;

            _xs.RemoveCell(kvid2Panel.X);
            _ys.RemoveCell(kvid2Panel.Y);
            _zs.RemoveCell(kvid2Panel.Z);
        }

        public List<(float? x, float? y, float? z)> GetVoltages()
        {
            var result = new List<(float? x, float? y, float? z)>();

            foreach (var panel in _kvid2Panels)
                result.Add((panel.X.NullableFloatValue, panel.Y.NullableFloatValue, panel.Z.NullableFloatValue));

            return result;
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