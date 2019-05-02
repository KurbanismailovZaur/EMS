using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Management.Tables;
using System.Linq;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;
using System.Collections.ObjectModel;

namespace UI.Tables.Concrete
{
    public class ConnectorTypesTable : Table
    {
        public class ConnectorTypePanel : Panel
        {
            public Cell Code { get; set; }

            public Cell Type { get; set; }

            public ConnectorTypePanel(Cell code, Cell type)
            {
                Code = code;
                Type = type;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(Code.gameObject);
                UnityObject.Destroy(Type.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "Code":
                        return Code;
                    case "Type":
                        return Type;
                    default:
                        throw new ArgumentException($"No cell with name \"{ name }\"");
                }
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                throw new ArgumentException($"No reference cell with name \"{ name }\"");
            }

            public ConnectorType ToConnectorType()
            {
                return new ConnectorType
                {
                    Code = Code.StringValue,
                    Type = Type.NullableStringValue,
                };
            }
        }

        [SerializeField]
        private Column _codes;

        [SerializeField]
        private Column _types;

        private List<ConnectorTypePanel> _connectorTypePanels = new List<ConnectorTypePanel>();

        public override string RemoveCellName => "Code";

        public ReadOnlyCollection<ConnectorTypePanel> ConnectorTypePanels => new ReadOnlyCollection<ConnectorTypePanel>(_connectorTypePanels);

        public void AddConnectorTypes(Action<Cell> cellClickHandler)
        {
            foreach (var connectorType in TableDataManager.Instance.ConnectorTypes)
                Add(connectorType.Code, connectorType.Type, cellClickHandler);
        }

        private void Add(string code, string type, Action<Cell> cellClickHandler)
        {
            var codeCell = Cell.Factory.CreateUnique(_cellPrefab, code, _codes, cellClickHandler);
            var typeCell = Cell.Factory.Create(_cellPrefab, type, true, _types, cellClickHandler);

            var panel = new ConnectorTypePanel(codeCell, typeCell);
            _connectorTypePanels.Add(panel);

            AddPanelToColumns(panel);

            Added.Invoke(panel);
        }

        public override void AddEmpty(Action<Cell> cellClickHandler)
        {
            Add(GetNextCode(), null, cellClickHandler);
        }

        private string GetNextCode() => $"Р{_connectorTypePanels.Max(p => int.Parse(p.Code.StringValue.Substring(1))) + 1}";

        public override void Clear()
        {
            foreach (var panel in _connectorTypePanels)
                panel.Destroy();

            _connectorTypePanels.Clear();
        }

        public override List<Panel> GetSafeRemovingPanels()
        {
            return _connectorTypePanels.Cast<Panel>().ToList();
        }

        public override void Remove(Panel panel)
        {
            if (!_connectorTypePanels.Contains(panel)) return;

            _connectorTypePanels.Remove((ConnectorTypePanel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var connectorTypePanel = (ConnectorTypePanel)panel;

            _codes.AddCell(connectorTypePanel.Code);
            _types.AddCell(connectorTypePanel.Type);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var connectorTypePanel = (ConnectorTypePanel)panel;

            _codes.RemoveCell(connectorTypePanel.Code);
            _types.RemoveCell(connectorTypePanel.Type);
        }
    }
}