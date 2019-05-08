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
    public class KVID3TableHeader1 : Table
    {
        public class KVID3TableHeader1Panel : Panel
        {
            public ReferenceCell WireID { get; private set; }

            public KVID3TableHeader1Panel(ReferenceCell wireLenght)
            {
                WireID = wireLenght;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(WireID.gameObject);
            }

            public override Cell GetCell(string name)
            {
                throw new ArgumentException($"No cell with name \"{ name }\"");
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                switch (name)
                {
                    case "WireID":
                        return WireID;
                    default:
                        throw new ArgumentException($"No reference cell with name \"{ name }\"");
                }
            }
        }



        [SerializeField]
        private Column _wireIDs;


        public InputController InputController;

        public KVID3TableHeader1Panel Panel;


        private void Awake()
        {
            Panel = Add("-", Cell_Clicked);
        }

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add("-", cellClickHandler);

        private KVID3TableHeader1Panel Add(string id, Action<Cell> cellClickHandler)
        {
            var list = TableDataManager.Instance.WireMarks.Select(m => m.Code).ToList();
            var idCell = ReferenceCell.Factory.Create(_referenceCellPrefab, list, id, _wireIDs);

            var panel = new KVID3TableHeader1Panel(idCell);

            AddPanelToColumns(panel);

            return panel;
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvidPanel = (KVID3TableHeader1Panel)panel;

            _wireIDs.AddCell(kvidPanel.WireID);
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