
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using Management.Tables;
using System;
using UnityEngine.Events;
using System.Linq;
using Material = Management.Tables.Material;
using UnityObject = UnityEngine.Object;
using System.Collections.ObjectModel;

namespace UI.Tables.Concrete
{
    public class MaterialsTable : Table
    {
        public class MaterialPanel : Panel
        {
            public Cell Code { get; private set; }

            public Cell Name { get; private set; }

            public Cell Conductivity { get; private set; }

            public Cell MagneticPermeability { get; private set; }

            public Cell DielectricConstant { get; private set; }

            public MaterialPanel(Cell code, Cell name, Cell conductivity, Cell magneticPermeability, Cell dielectricConstant)
            {
                Code = code;
                Name = name;
                Conductivity = conductivity;
                MagneticPermeability = magneticPermeability;
                DielectricConstant = dielectricConstant;

                Code.Changed += Cell_Changed;
                Name.Changed += Cell_Changed;
                Conductivity.Changed += Cell_Changed;
                MagneticPermeability.Changed += Cell_Changed;
                DielectricConstant.Changed += Cell_Changed;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(Code.gameObject);
                UnityObject.Destroy(Name.gameObject);
                UnityObject.Destroy(Conductivity.gameObject);
                UnityObject.Destroy(MagneticPermeability.gameObject);
                UnityObject.Destroy(DielectricConstant.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "Code":
                        return Code;
                    case "Name":
                        return Name;
                    case "Conductivity":
                        return Conductivity;
                    case "MagneticPermeability":
                        return MagneticPermeability;
                    case "DielectricConstant":
                        return DielectricConstant;
                    default:
                        throw new ArgumentException($"No cell with name \"{ name }\"");
                }
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                throw new ArgumentException($"No reference cell with name \"{ name }\"");
            }

            public Material ToMaterial()
            {
                return new Material
                {
                    Code = Code.IntValue,
                    Name = Name.NullableStringValue,
                    Conductivity = Conductivity.NullableFloatValue,
                    MagneticPermeability = MagneticPermeability.NullableFloatValue,
                    DielectricConstant = DielectricConstant.NullableFloatValue
                };
            }

            #region Event handlers
            protected override void Cell_Changed(Cell cell)
            {
                if (cell == Code)
                    InvokeChangedEvent("Code");
                else if (cell == Name)
                    InvokeChangedEvent("Name");
                else if (cell == Conductivity)
                    InvokeChangedEvent("Conductivity");
                else if (cell == MagneticPermeability)
                    InvokeChangedEvent("MagneticPermeability");
                else if (cell == DielectricConstant)
                    InvokeChangedEvent("DielectricConstant");
            }
            #endregion
        }

        [Header("Columns")]
        [SerializeField]
        private Column _codes;

        [SerializeField]
        private Column _names;

        [SerializeField]
        private Column _conductivities;

        [SerializeField]
        private Column _magneticPermeabilities;

        [SerializeField]
        private Column _dielectricConstants;

        private List<MaterialPanel> _materialPanels = new List<MaterialPanel>();

        public ReadOnlyCollection<MaterialPanel> MaterialPanels => new ReadOnlyCollection<MaterialPanel>(_materialPanels);

        public override string RemoveCellName => "Name";

        [SerializeField]
        private WireMarksTable _wireMarksTable;

        public void AddMaterials(Action<Cell> cellClickHandler)
        {
            foreach (var material in TableDataManager.Instance.Materials)
                Add(material.Code, material.Name, material.Conductivity, material.MagneticPermeability, material.DielectricConstant, cellClickHandler);
        }

        private void Add(int code, string name, float? conductivity, float? magneticPermeability, float? dielectricConstant, Action<Cell> cellClickHandler)
        {
            var codeCell = Cell.Factory.CreateUnique(_cellPrefab, code, _codes, cellClickHandler);
            var nameCell = Cell.Factory.Create(_cellPrefab, name, true, _names, cellClickHandler);
            var conductivityCell = Cell.Factory.Create(_cellPrefab, conductivity, _conductivities, cellClickHandler);
            var magneticPermeabilityCell = Cell.Factory.Create(_cellPrefab, magneticPermeability, _magneticPermeabilities, cellClickHandler);
            var dielectricConstantCell = Cell.Factory.Create(_cellPrefab, dielectricConstant, _dielectricConstants, cellClickHandler);

            var panel = new MaterialPanel(codeCell, nameCell, conductivityCell, magneticPermeabilityCell, dielectricConstantCell);
            _materialPanels.Add(panel);

            AddPanelToColumns(panel);

            Added.Invoke(panel);
        }

        public override void AddEmpty(Action<Cell> cellClickHandler)
        {
            Add(GetNextCode(), null, null, null, null, cellClickHandler);
        }

        private int GetNextCode() => _materialPanels.Max(p => p.Code.IntValue) + 1;

        public override void Clear()
        {
            foreach (var panel in _materialPanels)
                panel.Destroy();

            _materialPanels.Clear();
        }

        public override List<Panel> GetSafeRemovingPanels()
        {
            List<Panel> panels = new List<Panel>();

            foreach (var materialPanel in _materialPanels)
            {
                bool noReference = true;

                foreach (var wireMarkPanel in _wireMarksTable.WireMarkPanels)
                {
                    if (wireMarkPanel.CoreMaterial.SelectedPanel == materialPanel || wireMarkPanel.Screen1Material.SelectedPanel == materialPanel || wireMarkPanel.Screen2Material.SelectedPanel == materialPanel)
                    {
                        noReference = false;
                        break;
                    }
                }

                if (noReference)
                    panels.Add(materialPanel);
            }

            return panels;

            //return _materialPanels.Where(p => _wireMarksTable.WireMarkPanels.All(wp => wp.CoreMaterial?.SelectedPanel != p && wp.Screen1Material?.SelectedPanel != p && wp.Screen2Material?.SelectedPanel != p))
            //    .Cast<Panel>()
            //    .ToList();
        }

        public override void Remove(Panel panel)
        {
            if (!_materialPanels.Contains(panel)) return;
            
            _materialPanels.Remove((MaterialPanel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();

            Removed.Invoke(panel);
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var materialPanel = (MaterialPanel)panel;

            _codes.AddCell(materialPanel.Code);
            _names.AddCell(materialPanel.Name);
            _conductivities.AddCell(materialPanel.Conductivity);
            _magneticPermeabilities.AddCell(materialPanel.MagneticPermeability);
            _dielectricConstants.AddCell(materialPanel.DielectricConstant);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var materialPanel = (MaterialPanel)panel;

            _codes.RemoveCell(materialPanel.Code);
            _names.RemoveCell(materialPanel.Name);
            _conductivities.RemoveCell(materialPanel.Conductivity);
            _magneticPermeabilities.RemoveCell(materialPanel.MagneticPermeability);
            _dielectricConstants.RemoveCell(materialPanel.DielectricConstant);
        }
    }
}