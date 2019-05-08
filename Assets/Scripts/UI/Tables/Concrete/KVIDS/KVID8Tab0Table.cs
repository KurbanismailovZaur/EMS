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
    public class KVID8Tab0Table : Table
    {
        public class KVID8Tab0Panel : Panel
        {
            public RemoveButton RemoveButton { get; private set; }

            public ReferenceCell ID { get; private set; }

            public Cell MaxVoltage { get; private set; }

            public Cell FrequencyMin { get; private set; }

            public Cell FrequencyMax { get; private set; }


            public KVID8Tab0Panel(RemoveButton removeButton, ReferenceCell id, Cell maxVoltage, Cell frequencyMin, Cell frequencyMax)
            {
                RemoveButton = removeButton;
                ID = id;
                MaxVoltage = maxVoltage;
                FrequencyMin = frequencyMin;
                FrequencyMax = frequencyMax;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(RemoveButton.gameObject);
                UnityObject.Destroy(ID.gameObject);
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
        private Column _maxVoltages;

        [SerializeField]
        private Column _frequencyMins;

        [SerializeField]
        private Column _frequencyMaxs;

        #endregion

        private List<KVID8Tab0Panel> _kvid8Panels = new List<KVID8Tab0Panel>();

        private int _nextCodeIndex;

        #region Columns
        public Column Removes => _removes;

        public Column IDs => _ids;

        public Column MaxVoltages => _maxVoltages;

        public Column FrequencyMins => _frequencyMins;

        public Column FrequencyMaxs => _frequencyMaxs;
        #endregion

        public override Panel AddEmpty(Action<Cell> cellClickHandler) => Add("-", 0f, 0, 0, cellClickHandler);

        private KVID8Tab0Panel Add(string id, float maxVoltage, int frequencyMin, int frequencyMax, Action<Cell> cellClickHandler)
        {
            var removeButton = RemoveButton.Factory.Create(_removeButtonPrefab, _removes.transform, RemoveButton_Clicked);
            var mvCell = Cell.Factory.Create(_cellPrefab, maxVoltage, _maxVoltages, cellClickHandler);
            var fMinCell = Cell.Factory.Create(_cellPrefab, frequencyMin, _frequencyMins, cellClickHandler);
            var fMaxCell = Cell.Factory.Create(_cellPrefab, frequencyMax, _frequencyMaxs, cellClickHandler);


            // reference cells
            var list =  CalculationsManager.Instance.ElectricFieldStrenght.Points.Select(p => p.Code).ToList();
            var idCell = ReferenceCell.Factory.Create(_referenceCellPrefab, list, id, _ids);





            var panel = new KVID8Tab0Panel(removeButton, idCell, mvCell, fMinCell, fMaxCell);
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
            if (!_kvid8Panels.Contains((KVID8Tab0Panel)panel)) return;

            _kvid8Panels.Remove((KVID8Tab0Panel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();

            Removed.Invoke(panel);
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var kvid8Panel = (KVID8Tab0Panel)panel;

            _ids.AddCell(kvid8Panel.ID);
            _maxVoltages.AddCell(kvid8Panel.MaxVoltage);
            _frequencyMins.AddCell(kvid8Panel.FrequencyMin);
            _frequencyMaxs.AddCell(kvid8Panel.FrequencyMax);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var kvid8Panel = (KVID8Tab0Panel)panel;

            _ids.RemoveCell(kvid8Panel.ID);
            _maxVoltages.RemoveCell(kvid8Panel.MaxVoltage);
            _frequencyMins.RemoveCell(kvid8Panel.FrequencyMin);
            _frequencyMaxs.RemoveCell(kvid8Panel.FrequencyMax);
        }

        public List<(string pointID, float maxVoltage, int fMin, int fMax)> GetPanelsData()
        {
            var result = new List<(string pointID, float maxVoltage, int fMin, int fMax)>();


            foreach (var panel in _kvid8Panels)
                result.Add((panel.ID.GetSelectedOptionName(), panel.MaxVoltage.FloatValue, panel.FrequencyMin.IntValue, panel.FrequencyMax.IntValue));

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