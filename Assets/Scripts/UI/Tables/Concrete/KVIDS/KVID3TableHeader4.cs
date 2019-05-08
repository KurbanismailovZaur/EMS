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
using Management.Tables;
using System.Linq;

namespace UI.Tables.Concrete.KVIDS
{
    public class KVID3TableHeader4 : Table
    {
        public class KVID3TableHeader4Panel : Panel
        {
            public ReferenceCell I { get; private set; }
            public ReferenceCell P { get; private set; }

            public KVID3TableHeader4Panel(ReferenceCell i, ReferenceCell p)
            {
                I = i;
                P = p;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(I.gameObject);
                UnityObject.Destroy(P.gameObject);
            }

            public override Cell GetCell(string name)
            {
                throw new ArgumentException($"No cell with name \"{ name }\"");
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                switch (name)
                {
                    case "I":
                        return I;
                    case "P":
                        return P;
                    default:
                        throw new ArgumentException($"No reference cell with name \"{ name }\"");
                }
            }
        }



        [SerializeField]
        private Column _is;

        [SerializeField]
        private Column _ps;


        public InputController InputController;

        public KVID3TableHeader4Panel Panel;


        private void Awake()
        {
            Panel = Add("-", "-", Cell_Clicked);
        }

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add("-", "-", cellClickHandler);

        private KVID3TableHeader4Panel Add(string i, string p, Action<Cell> cellClickHandler)
        {
            var list = TableDataManager.Instance.KVID5Data.Select(d => d.code).ToList();

            var iCell = ReferenceCell.Factory.Create(_referenceCellPrefab, list, i, _is);
            var pCell = ReferenceCell.Factory.Create(_referenceCellPrefab, list, p, _ps);

            var panel = new KVID3TableHeader4Panel(iCell, pCell);

            AddPanelToColumns(panel);

            return panel;
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvidPanel = (KVID3TableHeader4Panel)panel;

            _is.AddCell(kvidPanel.I);
            _ps.AddCell(kvidPanel.P);
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