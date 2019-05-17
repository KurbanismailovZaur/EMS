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

    public class KVID5Table : Table
    {
        public class KVID5Panel : Panel
        {
            public RemoveButton RemoveButton { get; private set; }

            public Cell Code { get; private set; }

            public Cell X { get; private set; }

            public Cell Y { get; private set; }

            public Cell Z { get; private set; }

            public Cell Type { get; private set; }

            public Cell InnerResist { get; private set; }

            public Cell OperatingVoltage { get; private set; }

            public Cell OperatingFrequensy { get; private set; }

            public ReferenceCell BlockBA { get; private set; }

            public Cell ConnectorType { get; private set; }





            public KVID5Panel(RemoveButton removeButton, Cell code, Cell x, Cell y, Cell z, Cell t, Cell iR, Cell oV, Cell oF, ReferenceCell bBA, Cell connectorType)
            {
                RemoveButton = removeButton;
                Code = code;
                X = x;
                Y = y;
                Z = z;
                Type = t;
                InnerResist = iR;
                OperatingVoltage = oV;
                OperatingFrequensy = oF;
                BlockBA = bBA;
                ConnectorType = connectorType;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(RemoveButton.gameObject);
                UnityObject.Destroy(Code.gameObject);
                UnityObject.Destroy(X.gameObject);
                UnityObject.Destroy(Y.gameObject);
                UnityObject.Destroy(Z.gameObject);
                UnityObject.Destroy(Type.gameObject);
                UnityObject.Destroy(InnerResist.gameObject);
                UnityObject.Destroy(OperatingVoltage.gameObject);
                UnityObject.Destroy(OperatingFrequensy.gameObject);
                UnityObject.Destroy(BlockBA.gameObject);
                UnityObject.Destroy(ConnectorType.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "Code":
                        return Code;
                    case "X":
                        return X;
                    case "Y":
                        return Y;
                    case "Z":
                        return Z;
                    case "Type":
                        return Type;
                    case "InnerResist":
                        return InnerResist;
                    case "OperatingVoltage":
                        return OperatingVoltage;
                    case "OperatingFrequensy":
                        return OperatingFrequensy;
                    case "ConnectorType":
                        return ConnectorType;
                    default:
                        throw new ArgumentException($"No cell with name \"{ name }\"");
                }
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                switch (name)
                {
                    case "BlockBA":
                        return BlockBA;
                    default:
                        throw new ArgumentException($"No reference cell with name \"{ name }\"");
                }
            }
        }

        [SerializeField]
        private RemoveButton _removeButtonPrefab;

        #region Columns
        [Header("Columns")]
        [SerializeField]
        private Column _removes;

        [SerializeField]
        private Column _codes;

        [SerializeField]
        private Column _xs;

        [SerializeField]
        private Column _ys;

        [SerializeField]
        private Column _zs;

        [SerializeField]
        private Column _types;

        [SerializeField]
        private Column _innerResists;

        [SerializeField]
        private Column _operatingVoltages;

        [SerializeField]
        private Column _operatingFrequensies;

        [SerializeField]
        private Column _blockBAs;

        [SerializeField]
        private Column _connectorTypes;
        #endregion

        private List<KVID5Panel> _kvid5Panels = new List<KVID5Panel>();

        private int _nextCodeIndex;

        #region Columns
        public Column Removes => _removes;

        public Column Codes => _xs;

        public Column Xs => _xs;

        public Column Ys => _ys;

        public Column Zs => _zs;

        public Column Types => _types;

        public Column InnerResists => _innerResists;

        public Column OperatingVoltages => _operatingVoltages;

        public Column OperatingFrequensies => _operatingFrequensies;

        public Column BlockBAs => _blockBAs;

        public Column ConnectorTypes => _connectorTypes;
        #endregion

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add(GetNextCode(), 0f, 0f, 0f, "И", null, null, null, "", "", cellClickHandler);

        private KVID5Panel Add(string code, float x, float y, float z, string type, int? iR, int? oV, int? oF, string blockBA, string connectorType, Action<Cell> cellClickHandler)
        {
            var removeButton = RemoveButton.Factory.Create(_removeButtonPrefab, _removes.transform, RemoveButton_Clicked);
            var codeCell = Cell.Factory.CreateUnique(_cellPrefab, code, _codes, cellClickHandler);
            var xCell = Cell.Factory.Create(_cellPrefab, x, _xs, cellClickHandler);
            var yCell = Cell.Factory.Create(_cellPrefab, y, _ys, cellClickHandler);
            var zCell = Cell.Factory.Create(_cellPrefab, z, _zs, cellClickHandler);
            var tCell = Cell.Factory.Create(_cellPrefab, type, false, _types, cellClickHandler, "И", "П", "У");
            var irCell = Cell.Factory.Create(_cellPrefab, iR, _innerResists, cellClickHandler);
            var ovCell = Cell.Factory.Create(_cellPrefab, oV, _operatingVoltages, cellClickHandler);
            var ofCell = Cell.Factory.Create(_cellPrefab, oF, _operatingFrequensies, cellClickHandler);
            var ctCell = Cell.Factory.Create(_cellPrefab, connectorType, true, _connectorTypes, cellClickHandler);

            // reference cells
            var list = TableDataManager.Instance.KVID2Data.Select(d => d.tabName ?? "-").ToList();
            var bbaCell = ReferenceCell.Factory.Create(_referenceCellPrefab, list, blockBA, _blockBAs);





            var panel = new KVID5Panel(removeButton, codeCell, xCell, yCell, zCell, tCell, irCell, ovCell, ofCell, bbaCell, ctCell);
            _kvid5Panels.Add(panel);

            removeButton.Panel = panel;
            AddPanelToColumns(panel);

            Added.Invoke(panel);

            return panel;
        }

        private string GetNextCode()
        {
            while (_kvid5Panels.Find(p => p.Code.StringValue == $"Пт{_nextCodeIndex}") != null) _nextCodeIndex += 1;

            return $"Пт{_nextCodeIndex}";
        }

        public override void Clear()
        {
            foreach (var panel in _kvid5Panels)
                panel.Destroy();

            _kvid5Panels.Clear();
        }

        public override void Remove(Panel panel)
        {
            if (!_kvid5Panels.Contains((KVID5Panel)panel)) return;

            _kvid5Panels.Remove((KVID5Panel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();

            Removed.Invoke(panel);
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvid5Panel = (KVID5Panel)panel;

            _codes.AddCell(kvid5Panel.Code);
            _xs.AddCell(kvid5Panel.X);
            _ys.AddCell(kvid5Panel.Y);
            _zs.AddCell(kvid5Panel.Z);

            _types.AddCell(kvid5Panel.Type);
            _innerResists.AddCell(kvid5Panel.InnerResist);
            _operatingVoltages.AddCell(kvid5Panel.OperatingVoltage);
            _operatingFrequensies.AddCell(kvid5Panel.OperatingFrequensy);
            _connectorTypes.AddCell(kvid5Panel.ConnectorType);

            // reference cells
            _blockBAs.AddCell(kvid5Panel.BlockBA);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var kvid5Panel = (KVID5Panel)panel;

            _codes.RemoveCell(kvid5Panel.Code);
            _xs.RemoveCell(kvid5Panel.X);
            _ys.RemoveCell(kvid5Panel.Y);
            _zs.RemoveCell(kvid5Panel.Z);

            _types.RemoveCell(kvid5Panel.Type);
            _innerResists.RemoveCell(kvid5Panel.InnerResist);
            _operatingVoltages.RemoveCell(kvid5Panel.OperatingVoltage);
            _operatingFrequensies.RemoveCell(kvid5Panel.OperatingFrequensy);
            _connectorTypes.RemoveCell(kvid5Panel.ConnectorType);

            // reference cells
            _blockBAs.RemoveCell(kvid5Panel.BlockBA);
        }

        public List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> GetPanelsData(List<string> usableKvid2Tabs)
        {
            var result = new List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)>();

            foreach (var panel in _kvid5Panels)
            {
                if (!usableKvid2Tabs.Contains(panel.BlockBA.GetSelectedOptionName()))
                    usableKvid2Tabs.Add(panel.BlockBA.GetSelectedOptionName());

                result.Add((panel.Code.StringValue, new Vector3(panel.X.FloatValue, panel.Y.FloatValue, panel.Z.FloatValue), panel.Type.StringValue, panel.InnerResist.NullableIntValue, panel.OperatingVoltage.NullableIntValue, panel.OperatingFrequensy.NullableIntValue, panel.BlockBA.GetSelectedOptionName(), panel.ConnectorType.NullableStringValue));
            }

            return result;
        }

        #region Event handlers
        private void RemoveButton_Clicked(RemoveButton removeButton, Panel panel)
        {
            if (TableDataManager.Instance.IsUsableKVID5Row(((KVID5Panel)panel).Code.StringValue)) return;

            Remove(panel);
            Destroy(removeButton.gameObject);
        }
        #endregion
    }

}