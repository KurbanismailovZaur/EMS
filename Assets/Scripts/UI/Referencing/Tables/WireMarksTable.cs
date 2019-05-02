using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Management.Referencing;
using System.Linq;
using Material = Management.Referencing.Material;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;
using System.Collections.ObjectModel;

namespace UI.Referencing.Tables
{
    public class WireMarksTable : Table
    {
        public class WireMarkPanel : Panel
        {
            public Cell Code { get; set; }

            public Cell Type { get; set; }

            public ReferenceCell CoreMaterial { get; set; }

            public Cell CoreDiameter { get; set; }

            public ReferenceCell Screen1Material { get; set; }

            public Cell Screen1InnerRadius { get; set; }

            public Cell Screen1Thresold { get; set; }

            public Cell Screen1IsolationMaterial { get; set; }

            public ReferenceCell Screen2Material { get; set; }

            public Cell Screen2InnerRadius { get; set; }

            public Cell Screen2Thresold { get; set; }

            public Cell Screen2IsolationMaterial { get; set; }

            public Cell CrossSectionDiameter { get; set; }

            public WireMarkPanel(Cell code, Cell type, ReferenceCell coreMaterial, Cell coreDiameter, ReferenceCell screen1Material, Cell screen1InnerRadius, Cell screen1Thresold, Cell screen1IsolationMaterial, ReferenceCell screen2Material, Cell screen2InnerRadius, Cell screen2Thresold, Cell screen2IsolationMaterial, Cell crossSectionDiameter)
            {
                Code = code;
                Type = type;
                CoreMaterial = coreMaterial;
                CoreDiameter = coreDiameter;
                Screen1Material = screen1Material;
                Screen1InnerRadius = screen1InnerRadius;
                Screen1Thresold = screen1Thresold;
                Screen1IsolationMaterial = screen1IsolationMaterial;
                Screen2Material = screen2Material;
                Screen2InnerRadius = screen2InnerRadius;
                Screen2Thresold = screen2Thresold;
                Screen2IsolationMaterial = screen2IsolationMaterial;
                CrossSectionDiameter = crossSectionDiameter;
            }

            public override void Destroy()
            {
                UnityObject.Destroy(Code.gameObject);
                UnityObject.Destroy(Type.gameObject);
                UnityObject.Destroy(CoreMaterial.gameObject);
                UnityObject.Destroy(CoreDiameter.gameObject);
                UnityObject.Destroy(Screen1Material.gameObject);
                UnityObject.Destroy(Screen1InnerRadius.gameObject);
                UnityObject.Destroy(Screen1Thresold.gameObject);
                UnityObject.Destroy(Screen1IsolationMaterial.gameObject);
                UnityObject.Destroy(Screen2Material.gameObject);
                UnityObject.Destroy(Screen2InnerRadius.gameObject);
                UnityObject.Destroy(Screen2Thresold.gameObject);
                UnityObject.Destroy(Screen2IsolationMaterial.gameObject);
                UnityObject.Destroy(CrossSectionDiameter.gameObject);
            }

            public override Cell GetCell(string name)
            {
                switch (name)
                {
                    case "Code":
                        return Code;
                    case "Type":
                        return Type;
                    case "CoreDiameter":
                        return CoreDiameter;
                    case "Screen1InnerRadius":
                        return Screen1InnerRadius;
                    case "Screen1Thresold":
                        return Screen1Thresold;
                    case "Screen1IsolationMaterial":
                        return Screen1IsolationMaterial;
                    case "Screen2InnerRadius":
                        return Screen2InnerRadius;
                    case "Screen2Thresold":
                        return Screen2Thresold;
                    case "Screen2IsolationMaterial":
                        return Screen2IsolationMaterial;
                    case "CrossSectionDiameter":
                        return CrossSectionDiameter;
                    default:
                        throw new ArgumentException($"No cell with name \"{ name }\"");
                }
            }

            public override ReferenceCell GetReferenceCell(string name)
            {
                switch (name)
                {
                    case "CoreMaterial":
                        return CoreMaterial;
                    case "Screen1Material":
                        return Screen1Material;
                    case "Screen2Material":
                        return Screen2Material;
                    default:
                        throw new ArgumentException($"No reference cell with name \"{ name }\"");
                }
            }

            public WireMark ToWireMark(List<Material> materials)
            {
                var coreMaterial = materials.Find(m => m.Code == CoreMaterial.SelectedPanel?.GetCell("Code").IntValue);
                var screen1Material = materials.Find(m => m.Code == Screen1Material.SelectedPanel?.GetCell("Code").IntValue);
                var screen2Material = materials.Find(m => m.Code == Screen2Material.SelectedPanel?.GetCell("Code").IntValue);

                return new WireMark
                {
                    Code = Code.StringValue,
                    Type = Type.NullableStringValue,
                    CoreMaterial = coreMaterial,
                    CoreDiameter = CoreDiameter.NullableFloatValue,
                    Screen1 = new WireMark.Screen
                    {
                        Material = screen1Material,
                        InnerRadius = Screen1InnerRadius.NullableFloatValue,
                        Thresold = Screen1Thresold.NullableFloatValue,
                        IsolationMaterial = Screen1IsolationMaterial.NullableStringValue
                    },
                    Screen2 = new WireMark.Screen
                    {
                        Material = screen2Material,
                        InnerRadius = Screen2InnerRadius.NullableFloatValue,
                        Thresold = Screen2Thresold.NullableFloatValue,
                        IsolationMaterial = Screen2IsolationMaterial.NullableStringValue
                    },
                    CrossSectionDiameter = CrossSectionDiameter.NullableFloatValue
                };
            }
        }

        [SerializeField]
        private Column _codes;

        [SerializeField]
        private Column _types;

        [SerializeField]
        private Column _coreMaterials;

        [SerializeField]
        private Column _coreDiameters;

        #region Screen1
        [SerializeField]
        private Column _screen1Materials;

        [SerializeField]
        private Column _screen1InnerRadii;

        [SerializeField]
        private Column _screen1Thresolds;

        [SerializeField]
        private Column _screen1isolationMaterials;
        #endregion

        #region Screen2
        [SerializeField]
        private Column _screen2Materials;

        [SerializeField]
        private Column _screen2InnerRadii;

        [SerializeField]
        private Column _screen2Thresolds;

        [SerializeField]
        private Column _screen2isolationMaterials;
        #endregion

        [SerializeField]
        private Column _crossSectionDiameters;

        private List<WireMarkPanel> _wireMarksPanels = new List<WireMarkPanel>();

        [Header("Linked Tables")]
        [SerializeField]
        private MaterialsTable _materialsTable;

        public ReadOnlyCollection<WireMarkPanel> WireMarkPanels => new ReadOnlyCollection<WireMarkPanel>(_wireMarksPanels);

        public override string RemoveCellName => "Code";

        public void AddWireMarks(Action<Cell> cellClickHandler)
        {
            var materialPanels = _materialsTable.MaterialPanels;

            foreach (var wireMark in ReferenceManager.Instance.WireMarks)
                Add(wireMark.Code, wireMark.Type, wireMark.CoreMaterial, wireMark.CoreDiameter, wireMark.Screen1.Material, wireMark.Screen1.InnerRadius, wireMark.Screen1.Thresold, wireMark.Screen1.IsolationMaterial, wireMark.Screen2.Material, wireMark.Screen2.InnerRadius, wireMark.Screen2.Thresold, wireMark.Screen2.IsolationMaterial, wireMark.CrossSectionDiameter, cellClickHandler);
        }

        public override void AddEmpty(Action<Cell> cellClickHandler)
        {
            var materialPanels = _materialsTable.MaterialPanels;

            Add(GetNextCode(), null, null, null, null, null, null, null, null, null, null, null, null, cellClickHandler);
        }

        private string GetNextCode() => $"м{_wireMarksPanels.Max(p => int.Parse(p.Code.StringValue.Substring(1))) + 1}";

        private void Add(string code, string type, Material coreMaterial, float? coreDiameter, Material screen1Material, float? screen1InnerRadius, float? screen1Thresold, string screen1isolationMaterial, Material screen2Material, float? screen2InnerRadius, float? screen2Thresold, string screen2isolationMaterial, float? crossSectionDiameter, Action<Cell> cellClickHandler)
        {
            var materialPanels = _materialsTable.MaterialPanels;
            var nullableMaterialPanels = Enumerable.Repeat((MaterialsTable.MaterialPanel)null, 1).Concat(materialPanels).ToList();

            var codeCell = Cell.Factory.CreateUnique(_cellPrefab, code, _codes, cellClickHandler);
            var typeCell = Cell.Factory.Create(_cellPrefab, type, true, _types, cellClickHandler);

            var selected = nullableMaterialPanels.IndexOf(nullableMaterialPanels.First(p => p?.Code.IntValue == coreMaterial?.Code));

            var coreMaterialCell = ReferenceCell.Factory.Create(_referenceCellPrefab, _materialsTable, nullableMaterialPanels, selected, "Name", _coreMaterials);
            var coreDiameterCell = Cell.Factory.Create(_cellPrefab, coreDiameter, _coreDiameters, cellClickHandler);

            selected = nullableMaterialPanels.IndexOf(nullableMaterialPanels.First(p => p?.Code.IntValue == screen1Material?.Code));

            var screen1MaterialCell = ReferenceCell.Factory.Create(_referenceCellPrefab, _materialsTable, nullableMaterialPanels, selected, "Name", _screen1Materials);
            var screen1InnerRadiusCell = Cell.Factory.Create(_cellPrefab, screen1InnerRadius, _screen1InnerRadii, cellClickHandler);
            var screen1ThresoldCell = Cell.Factory.Create(_cellPrefab, screen1Thresold, _screen1Thresolds, cellClickHandler);
            var screen1IsolationMaterialCell = Cell.Factory.Create(_cellPrefab, screen1isolationMaterial, true, _screen1isolationMaterials, cellClickHandler);

            selected = nullableMaterialPanels.IndexOf(nullableMaterialPanels.First(p => p?.Code.IntValue == screen2Material?.Code));

            var screen2MaterialCell = ReferenceCell.Factory.Create(_referenceCellPrefab, _materialsTable, nullableMaterialPanels, selected, "Name", _screen2Materials);
            var screen2InnerRadiusCell = Cell.Factory.Create(_cellPrefab, screen2InnerRadius, _screen2InnerRadii, cellClickHandler);
            var screen2ThresoldCell = Cell.Factory.Create(_cellPrefab, screen2Thresold, _screen2Thresolds, cellClickHandler);
            var screen2IsolationMaterialCell = Cell.Factory.Create(_cellPrefab, screen2isolationMaterial, true, _screen2isolationMaterials, cellClickHandler);

            var crossSectionDiameterCell = Cell.Factory.Create(_cellPrefab, crossSectionDiameter, _crossSectionDiameters, cellClickHandler);

            var panel = new WireMarkPanel(codeCell, typeCell, coreMaterialCell, coreDiameterCell, screen1MaterialCell, screen1InnerRadiusCell, screen1ThresoldCell, screen1IsolationMaterialCell, screen2MaterialCell, screen2InnerRadiusCell, screen2ThresoldCell, screen2IsolationMaterialCell, crossSectionDiameterCell);
            _wireMarksPanels.Add(panel);

            AddPanelToColumns(panel);

            Added.Invoke(panel);
        }

        public override void Clear()
        {
            foreach (var panel in _wireMarksPanels)
                panel.Destroy();

            _wireMarksPanels.Clear();
        }

        public override List<Panel> GetSafeRemovingPanels()
        {
            return _wireMarksPanels.Cast<Panel>().ToList();
        }

        public override void Remove(Panel panel)
        {
            if (!_wireMarksPanels.Contains(panel)) return;

            _wireMarksPanels.Remove((WireMarkPanel)panel);
            RemovePanelFromColumns(panel);
            panel.Destroy();
        }

        protected override void AddPanelToColumns(Panel panel)
        {
            var wireMarkPanel = (WireMarkPanel)panel;

            _codes.AddCell(wireMarkPanel.Code);
            _types.AddCell(wireMarkPanel.Type);
            _coreMaterials.AddCell(wireMarkPanel.CoreMaterial);
            _coreDiameters.AddCell(wireMarkPanel.CoreDiameter);
            _screen1Materials.AddCell(wireMarkPanel.Screen1Material);
            _screen1InnerRadii.AddCell(wireMarkPanel.Screen1InnerRadius);
            _screen1Thresolds.AddCell(wireMarkPanel.Screen1Thresold);
            _screen1isolationMaterials.AddCell(wireMarkPanel.Screen1IsolationMaterial);
            _screen2Materials.AddCell(wireMarkPanel.Screen2Material);
            _screen2InnerRadii.AddCell(wireMarkPanel.Screen2InnerRadius);
            _screen2Thresolds.AddCell(wireMarkPanel.Screen2Thresold);
            _screen2isolationMaterials.AddCell(wireMarkPanel.Screen2IsolationMaterial);
            _crossSectionDiameters.AddCell(wireMarkPanel.CrossSectionDiameter);
        }

        protected override void RemovePanelFromColumns(Panel panel)
        {
            var wireMarkPanel = (WireMarkPanel)panel;

            _codes.RemoveCell(wireMarkPanel.Code);
            _types.RemoveCell(wireMarkPanel.Type);
            _coreMaterials.RemoveCell(wireMarkPanel.CoreMaterial);
            _coreDiameters.RemoveCell(wireMarkPanel.CoreDiameter);
            _screen1Materials.RemoveCell(wireMarkPanel.Screen1Material);
            _screen1InnerRadii.RemoveCell(wireMarkPanel.Screen1InnerRadius);
            _screen1Thresolds.RemoveCell(wireMarkPanel.Screen1Thresold);
            _screen1isolationMaterials.RemoveCell(wireMarkPanel.Screen1IsolationMaterial);
            _screen2Materials.RemoveCell(wireMarkPanel.Screen2Material);
            _screen2InnerRadii.RemoveCell(wireMarkPanel.Screen2InnerRadius);
            _screen2Thresolds.RemoveCell(wireMarkPanel.Screen2Thresold);
            _screen2isolationMaterials.RemoveCell(wireMarkPanel.Screen2IsolationMaterial);
            _crossSectionDiameters.RemoveCell(wireMarkPanel.CrossSectionDiameter);
        }
    }
}