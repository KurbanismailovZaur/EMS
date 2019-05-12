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
    public class KVID2TableHeader0 : Table
    {
        public class KVID2TableHeader0Panel : Panel
        {
            public Cell ProductName { get; private set; }

            public KVID2TableHeader0Panel(Cell wireLenght)
            {
                ProductName = wireLenght;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(ProductName.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "ProductName":
                        return ProductName;
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
        private Column _productName;


        public InputController InputController;

        public KVID2TableHeader0Panel Panel;


        private void Awake()
        {
            Panel = Add("-", Cell_Clicked);
        }

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add("-", cellClickHandler);

        private KVID2TableHeader0Panel Add(string productName, Action<Cell> cellClickHandler)
        {
            var productNameCell = Cell.Factory.Create(_cellPrefab, productName, false, _productName, cellClickHandler);

            var panel = new KVID2TableHeader0Panel(productNameCell);

            AddPanelToColumns(panel);

            return panel;
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvidPanel = (KVID2TableHeader0Panel)panel;

            _productName.AddCell(kvidPanel.ProductName);
        }

        public string GetProductName()
        {
            return Panel.ProductName.StringValue;
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