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
    public class KVID2TableHeader : Table
    {
        public class KVID2TableHeaderPanel : Panel
        {
            public Cell X { get; private set; }

            public Cell Y { get; private set; }

            public Cell Z { get; private set; }

            public KVID2TableHeaderPanel(Cell x, Cell y, Cell z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public override void Destroy()
            {
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
        private Column _xs;

        [SerializeField]
        private Column _ys;

        [SerializeField]
        private Column _zs;

        public InputController InputController;

        public KVID2TableHeaderPanel Panel;


        public Vector3 GetCenterPoint()
        {
            return new Vector3(Panel.X.FloatValue, Panel.Y.FloatValue, Panel.Z.FloatValue);
        }

        private void Awake()
        {
            Panel = Add(0, 0, 0, Cell_Clicked);
        }

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add(0, 0, 0, cellClickHandler);

        private KVID2TableHeaderPanel Add(float x, float y, float z, Action<Cell> cellClickHandler)
        {
            var xCell = Cell.Factory.Create(_cellPrefab, x, _xs, cellClickHandler);
            var yCell = Cell.Factory.Create(_cellPrefab, y, _ys, cellClickHandler);
            var zCell = Cell.Factory.Create(_cellPrefab, z, _zs, cellClickHandler);

            var panel = new KVID2TableHeaderPanel(xCell, yCell, zCell);

            AddPanelToColumns(panel);

            return panel;
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvid2Panel = (KVID2TableHeaderPanel)panel;

            _xs.AddCell(kvid2Panel.X);
            _ys.AddCell(kvid2Panel.Y);
            _zs.AddCell(kvid2Panel.Z);
        }


        protected void Cell_Clicked(Cell cell) => InputController.Edit(cell);

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Remove(Panel panel)
        {
            throw new NotImplementedException();
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            throw new NotImplementedException();
        }
    }
}