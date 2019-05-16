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
    public class KVID3Table : Table
    {
        public class KVID3Panel : Panel
        {
            public RemoveButton RemoveButton { get; private set; }

            public Cell X { get; private set; }

            public Cell Y { get; private set; }

            public Cell Z { get; private set; }

            public Cell Metallization1 { get; private set; }

            public Cell Metallization2 { get; private set; }

            public KVID3Panel(RemoveButton removeButton, Cell xs, Cell ys, Cell sz, Cell metallizations1, Cell metallizations2)
            {
                RemoveButton = removeButton;
                X = xs;
                Y = ys;
                Z = sz;
                Metallization1 = metallizations1;
                Metallization2 = metallizations2;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(RemoveButton.gameObject);
                UnityObject.Destroy(X.gameObject);
                UnityObject.Destroy(Y.gameObject);
                UnityObject.Destroy(Z.gameObject);
                UnityObject.Destroy(Metallization1.gameObject);
                UnityObject.Destroy(Metallization2.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "Xs":
                        return X;
                    case "Ys":
                        return Y;
                    case "Zs":
                        return Z;
                    case "Metallizations1":
                        return Metallization1;
                    case "Metallizations2":
                        return Metallization2;
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

        [SerializeField]
        private Column _metallizations1;

        [SerializeField]
        private Column _metallizations2;
        #endregion

        private List<KVID3Panel> _kvid3Panels = new List<KVID3Panel>();

        #region Columns
        public Column Removes => _removes;

        public Column Xs => _xs;

        public Column Ys => _ys;

        public Column Zs => _zs;

        public Column Metallizations1 => _metallizations1;

        public Column Metallizations2 => _metallizations2;
        #endregion

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add(0f, 0f, 0f, null, null, cellClickHandler);

        private KVID3Panel Add(float x, float y, float z, float? metallization1, float? metallization2, Action<Cell> cellClickHandler)
        {
            var removeButton = RemoveButton.Factory.Create(_removeButtonPrefab, _removes.transform, RemoveButton_Clicked);
            var xCell = Cell.Factory.Create(_cellPrefab, x, _xs, cellClickHandler);
            var yCell = Cell.Factory.Create(_cellPrefab, y, _ys, cellClickHandler);
            var zCell = Cell.Factory.Create(_cellPrefab, z, _zs, cellClickHandler);
            var metallization1Cell = Cell.Factory.Create(_cellPrefab, metallization1, _metallizations1, cellClickHandler);
            var metallization2Cell = Cell.Factory.Create(_cellPrefab, metallization2, _metallizations2, cellClickHandler);

            var panel = new KVID3Panel(removeButton, xCell, yCell, zCell, metallization1Cell, metallization2Cell);
            _kvid3Panels.Add(panel);

            removeButton.Panel = panel;
            AddPanelToColumns(panel);

            Added.Invoke(panel);

            return panel;
        }

        public override void Clear()
        {
            foreach (var panel in _kvid3Panels)
                panel.Destroy();

            _kvid3Panels.Clear();
        }

        public override void Remove(Panel panel)
        {
            if (!_kvid3Panels.Contains((KVID3Panel)panel)) return;

            _kvid3Panels.Remove((KVID3Panel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();

            Removed.Invoke(panel);
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvid3Panel = (KVID3Panel)panel;
            
            _xs.AddCell(kvid3Panel.X);
            _ys.AddCell(kvid3Panel.Y);
            _zs.AddCell(kvid3Panel.Z);
            _metallizations1.AddCell(kvid3Panel.Metallization1);
            _metallizations2.AddCell(kvid3Panel.Metallization2);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var kvid3Panel = (KVID3Panel)panel;

            _xs.RemoveCell(kvid3Panel.X);
            _ys.RemoveCell(kvid3Panel.Y);
            _zs.RemoveCell(kvid3Panel.Z);
            _metallizations1.RemoveCell(kvid3Panel.Metallization1);
            _metallizations2.RemoveCell(kvid3Panel.Metallization2);
        }

        public (string name, List<Wire.Point> points) GetWireDataFromPanels()
        {
            var points = new List<Wire.Point>();

            foreach (var panel in _kvid3Panels)
            {
                Vector3 position = new Vector3(panel.X.FloatValue, panel.Y.FloatValue, panel.Z.FloatValue);
                points.Add(new Wire.Point(position, panel.Metallization1.NullableFloatValue, panel.Metallization2.NullableFloatValue));
            }

            return (name, points);
        }

        #region Event handlers
        private void RemoveButton_Clicked(RemoveButton removeButton, Panel panel)
        {
            if (_kvid3Panels.Count == 2) return;

            Remove(panel);
        }
        #endregion
    }
}