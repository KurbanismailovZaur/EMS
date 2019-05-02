using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityObject = UnityEngine.Object;

namespace UI.Tables.Concrete.KVIDS
{
    public class KVID3Table : Table
    {
        public class KVID3Panel : Panel
        {
            public Cell Xs { get; private set; }

            public Cell Ys { get; private set; }

            public Cell Zs { get; private set; }

            public Cell Metallizations1 { get; private set; }

            public Cell Metallizations2 { get; private set; }

            public KVID3Panel(Cell xs, Cell ys, Cell sz, Cell metallizations1, Cell metallizations2)
            {
                Xs = xs;
                Ys = ys;
                Zs = Zs;
                Metallizations1 = metallizations1;
                Metallizations2 = metallizations2;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(Xs.gameObject);
                UnityObject.Destroy(Ys.gameObject);
                UnityObject.Destroy(Zs.gameObject);
                UnityObject.Destroy(Metallizations1.gameObject);
                UnityObject.Destroy(Metallizations2.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "Xs":
                        return Xs;
                    case "Ys":
                        return Ys;
                    case "Zs":
                        return Zs;
                    case "Metallizations1":
                        return Metallizations1;
                    case "Metallizations2":
                        return Metallizations2;
                    default:
                        throw new ArgumentException($"No cell with name \"{ name }\"");
                }
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                throw new ArgumentException($"No reference cell with name \"{ name }\"");
            }
        }

        #region Columns
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
        public Column Xs => _xs;

        public Column Ys => _ys;

        public Column Zs => _zs;

        public Column Metallizations1 => _metallizations1;

        public Column Metallizations2 => _metallizations2;
        #endregion

        public override string RemoveCellName => throw new NotImplementedException();

        public override void AddEmpty(Action<Cell> cellClickHandler) => Add(0f, 0f, 0f, null, null, cellClickHandler);

        private void Add(float x, float y, float z, float? metallization1, float? metallization2, Action<Cell> cellClickHandler)
        {
            var xCell = Cell.Factory.Create(_cellPrefab, x, _xs, cellClickHandler);
            var yCell = Cell.Factory.Create(_cellPrefab, y, _ys, cellClickHandler);
            var zCell = Cell.Factory.Create(_cellPrefab, z, _zs, cellClickHandler);
            var metallization1Cell = Cell.Factory.Create(_cellPrefab, metallization1, _metallizations1, cellClickHandler);
            var metallization2Cell = Cell.Factory.Create(_cellPrefab, metallization2, _metallizations2, cellClickHandler);

            var panel = new KVID3Panel(xCell, yCell, zCell, metallization1Cell, metallization2Cell);
            _kvid3Panels.Add(panel);

            AddPanelToColumns(panel);

            Added.Invoke(panel);
        }

        public override void Clear()
        {
            foreach (var panel in _kvid3Panels)
                panel.Destroy();

            _kvid3Panels.Clear();
        }

        public override List<Panel> GetSafeRemovingPanels()
        {
            throw new NotImplementedException();
        }

        public override void Remove(Panel panel)
        {
            throw new NotImplementedException();
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvid3Panel = (KVID3Panel)panel;

            _xs.AddCell(kvid3Panel.Xs);
            _ys.AddCell(kvid3Panel.Ys);
            _zs.AddCell(kvid3Panel.Zs);
            _metallizations1.AddCell(kvid3Panel.Metallizations1);
            _metallizations2.AddCell(kvid3Panel.Metallizations2);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var kvid3Panel = (KVID3Panel)panel;

            _xs.RemoveCell(kvid3Panel.Xs);
            _ys.RemoveCell(kvid3Panel.Ys);
            _zs.RemoveCell(kvid3Panel.Zs);
            _metallizations1.RemoveCell(kvid3Panel.Metallizations1);
            _metallizations2.RemoveCell(kvid3Panel.Metallizations2);
        }
    }
}