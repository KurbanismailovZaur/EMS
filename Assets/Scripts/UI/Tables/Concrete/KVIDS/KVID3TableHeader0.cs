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
    public class KVID3TableHeader0 : Table
    {
        public class KVID3TableHeader0Panel : Panel
        {
            public Cell WireLenght { get; private set; }

            public KVID3TableHeader0Panel(Cell wireLenght)
            {
                WireLenght = wireLenght;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(WireLenght.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "WireLenght":
                        return WireLenght;
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
        private Column _wireLenghts;


        public InputController InputController;

        public KVID3TableHeader0Panel Panel;


        private void Awake()
        {
            Panel = Add(0f, Cell_Clicked);
        }

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add(0f, cellClickHandler);

        private KVID3TableHeader0Panel Add(float lenght, Action<Cell> cellClickHandler)
        {
            var lenghtCell = Cell.Factory.Create(_cellPrefab, lenght, _wireLenghts, cellClickHandler);

            var panel = new KVID3TableHeader0Panel(lenghtCell);

            AddPanelToColumns(panel);

            return panel;
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvidPanel = (KVID3TableHeader0Panel)panel;

            _wireLenghts.AddCell(kvidPanel.WireLenght);
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