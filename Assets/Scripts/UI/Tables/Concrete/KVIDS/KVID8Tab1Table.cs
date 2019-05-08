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
using Management.Calculations;

namespace UI.Tables.Concrete.KVIDS
{
    public class KVID8Tab1Table : Table
    {
        public class KVID8Tab1Panel : Panel
        {
            public RemoveButton RemoveButton { get; private set; }

            public ReferenceCell ID { get; private set; }

            public ReferenceCell WireID { get; private set; }

            public Cell MaxVoltage { get; private set; }

            public Cell FrequencyMin { get; private set; }

            public Cell FrequencyMax { get; private set; }


            public KVID8Tab1Panel(RemoveButton removeButton, ReferenceCell id, ReferenceCell wireId, Cell maxVoltage, Cell frequencyMin, Cell frequencyMax)
            {
                RemoveButton = removeButton;
                ID = id;
                WireID = wireId;
                MaxVoltage = maxVoltage;
                FrequencyMin = frequencyMin;
                FrequencyMax = frequencyMax;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(RemoveButton.gameObject);
                UnityObject.Destroy(ID.gameObject);
                UnityObject.Destroy(WireID.gameObject);
                UnityObject.Destroy(MaxVoltage.gameObject);
                UnityObject.Destroy(FrequencyMin.gameObject);
                UnityObject.Destroy(FrequencyMax.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "MaxVoltage":
                        return MaxVoltage;
                    case "FrequencyMin":
                        return FrequencyMin;
                    case "FrequencyMax":
                        return FrequencyMax;
                    default:
                        throw new ArgumentException($"No cell with name \"{ name }\"");
                }
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                switch (name)
                {
                    case "WireID":
                        return WireID;
                    case "ID":
                        return ID;
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
        private Column _ids;

        [SerializeField]
        private Column _wireIds;

        [SerializeField]
        private Column _maxVoltages;

        [SerializeField]
        private Column _frequencyMins;

        [SerializeField]
        private Column _frequencyMaxs;

        #endregion

        private List<KVID8Tab1Panel> _kvid8Panels = new List<KVID8Tab1Panel>();

        private int _nextCodeIndex;

        #region Columns
        public Column Removes => _removes;

        public Column IDs => _ids;

        public Column WireIDs => _wireIds;

        public Column MaxVoltages => _maxVoltages;

        public Column FrequencyMins => _frequencyMins;

        public Column FrequencyMaxs => _frequencyMaxs;
        #endregion

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add("-", "-", 0f, 0, 0, cellClickHandler);

        private KVID8Tab1Panel Add(string id, string wireId, float maxVoltage, int frequencyMin, int frequencyMax, Action<Cell> cellClickHandler)
        {
            var removeButton = RemoveButton.Factory.Create(_removeButtonPrefab, _removes.transform, RemoveButton_Clicked);
            var mvCell = Cell.Factory.Create(_cellPrefab, maxVoltage, _maxVoltages, cellClickHandler);
            var fMinCell = Cell.Factory.Create(_cellPrefab, frequencyMin, _frequencyMins, cellClickHandler);
            var fMaxCell = Cell.Factory.Create(_cellPrefab, frequencyMax, _frequencyMaxs, cellClickHandler);


            // reference cells
            var list = TableDataManager.Instance.KVID5Data.Select(d => d.code).ToList();
            var idCell = ReferenceCell.Factory.Create(_referenceCellPrefab, list, id, _ids);

            var list1 = WiringManager.Instance.Wiring.Wires.Select(w => w.Name).ToList();
            var wireIdCell = ReferenceCell.Factory.Create(_referenceCellPrefab, list1, wireId, _wireIds);




            var panel = new KVID8Tab1Panel(removeButton, idCell, wireIdCell, mvCell, fMinCell, fMaxCell);
            _kvid8Panels.Add(panel);

            removeButton.Panel = panel;
            AddPanelToColumns(panel);

            Added.Invoke(panel);

            return panel;
        }

        public override void Clear()
        {
            foreach (var panel in _kvid8Panels)
                panel.Destroy();

            _kvid8Panels.Clear();
        }

        public override void Remove(Panel panel)
        {
            if (!_kvid8Panels.Contains((KVID8Tab1Panel)panel)) return;

            _kvid8Panels.Remove((KVID8Tab1Panel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();

            Removed.Invoke(panel);
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvid8Panel = (KVID8Tab1Panel)panel;

            _ids.AddCell(kvid8Panel.ID);
            _wireIds.AddCell(kvid8Panel.WireID);
            _maxVoltages.AddCell(kvid8Panel.MaxVoltage);
            _frequencyMins.AddCell(kvid8Panel.FrequencyMin);
            _frequencyMaxs.AddCell(kvid8Panel.FrequencyMax);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var kvid8Panel = (KVID8Tab1Panel)panel;

            _ids.RemoveCell(kvid8Panel.ID);
            _wireIds.RemoveCell(kvid8Panel.WireID);
            _maxVoltages.RemoveCell(kvid8Panel.MaxVoltage);
            _frequencyMins.RemoveCell(kvid8Panel.FrequencyMin);
            _frequencyMaxs.RemoveCell(kvid8Panel.FrequencyMax);
        }

        public List<(string pointID, string wireID, float maxVoltage, int fMin, int fMax)> GetPanelsData()
        {
            var result = new List<(string pointID, string wireID, float maxVoltage, int fMin, int fMax)>();


            foreach (var panel in _kvid8Panels)
                result.Add((panel.ID.GetSelectedOptionName(), panel.WireID.GetSelectedOptionName(), panel.MaxVoltage.FloatValue, panel.FrequencyMin.IntValue, panel.FrequencyMax.IntValue));

            return result;
        }

        #region Event handlers
        private void RemoveButton_Clicked(RemoveButton removeButton, Panel panel)
        {
            Remove(panel);
            Destroy(removeButton.gameObject);
        }
        #endregion
    }

}