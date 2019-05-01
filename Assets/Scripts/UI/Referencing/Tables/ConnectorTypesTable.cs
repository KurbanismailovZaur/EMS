using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Management.Referencing;
using System.Linq;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;

namespace UI.Referencing.Tables
{
    public class ConnectorTypesTable : Table
    {
        public class ConnectorTypePanel
        {
            public Cell Code { get; set; }

            public Cell Type { get; set; }

            public ConnectorTypePanel(Cell code, Cell type)
            {
                Code = code;
                Type = type;
            }

            public void Destroy()
            {
                UnityObject.Destroy(Code.gameObject);
                UnityObject.Destroy(Type.gameObject);
            }
        }

        [SerializeField]
        private RectTransform _codes;

        [SerializeField]
        private RectTransform _types;

        private List<ConnectorTypePanel> _connectorTypePanels = new List<ConnectorTypePanel>();
        
        public void AddConnectorTypes(Action<Cell> cellClickHandler)
        {
            foreach (var connectorType in ReferenceManager.Instance.ConnectorTypes)
                Add(connectorType.Code, connectorType.Type, cellClickHandler);
        }

        private void Add(string code, string type, Action<Cell> cellClickHandler)
        {
            var codeCell = Cell.Factory.Create(_cellPrefab, code, false, _codes, cellClickHandler);
            var typeCell = Cell.Factory.Create(_cellPrefab, type, true, _types, cellClickHandler);
            
            var panel = new ConnectorTypePanel(codeCell, typeCell);
            _connectorTypePanels.Add(panel);
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
        }
    }
}