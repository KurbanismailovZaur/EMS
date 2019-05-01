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

namespace UI.Referencing.Tables
{
    public class WireMarksTable : Table
    {
        public class WireMarkPanel
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

            public void Destroy()
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
        }

        [SerializeField]
        private RectTransform _codes;

        [SerializeField]
        private RectTransform _types;

        [SerializeField]
        private RectTransform _coreMaterials;

        [SerializeField]
        private RectTransform _coreDiameters;

        #region Screen1
        [SerializeField]
        private RectTransform _screen1Materials;

        [SerializeField]
        private RectTransform _screen1InnerRadii;

        [SerializeField]
        private RectTransform _screen1Thresolds;

        [SerializeField]
        private RectTransform _screen1isolationMaterials;
        #endregion

        #region Screen2
        [SerializeField]
        private RectTransform _screen2Materials;

        [SerializeField]
        private RectTransform _screen2InnerRadii;

        [SerializeField]
        private RectTransform _screen2Thresolds;

        [SerializeField]
        private RectTransform _screen2isolationMaterials;
        #endregion

        [SerializeField]
        private RectTransform _crossSectionDiameters;

        private List<WireMarkPanel> _wireMarksPanels = new List<WireMarkPanel>();

        [Header("Linked Tables")]
        [SerializeField]
        private MaterialsTable _materialsTable;

        public RectTransform CoreMaterials => _coreMaterials;

        public RectTransform Screen1Materials => _screen1Materials;

        public RectTransform Screen2Materials => _screen1Materials;

        public void AddWireMarks(Action<Cell> cellClickHandler)
        {
            var materialPanels = _materialsTable.Materials;

            foreach (var wireMark in ReferenceManager.Instance.WireMarks)
                Add(wireMark.Code, wireMark.Type, wireMark.CoreMaterial, wireMark.CoreDiameter, wireMark.Screen1.Material, wireMark.Screen1.InnerRadius, wireMark.Screen1.Thresold, wireMark.Screen1.IsolationMaterial, wireMark.Screen2.Material, wireMark.Screen2.InnerRadius, wireMark.Screen2.Thresold, wireMark.Screen2.IsolationMaterial, wireMark.CrossSectionDiameter, cellClickHandler);
        }

        public override void AddEmpty(Action<Cell> cellClickHandler)
        {
            var materialPanels = _materialsTable.Materials;

            //Add(GetNextCode(), null, materialPanels[0]., cellClickHandler);
        }

        private string GetNextCode() => $"м{_wireMarksPanels.Max(p => int.Parse(p.Code.StringValue.Substring(1))) + 1}";

        private void Add(string code, string type, Material coreMaterial, float coreDiameter, Material screen1Material, float? screen1InnerRadius, float? screen1Thresold, string screen1isolationMaterial, Material screen2Material, float? screen2InnerRadius, float? screen2Thresold, string screen2isolationMaterial, float crossSectionDiameter, Action<Cell> cellClickHandler)
        {
            var materialPanels = _materialsTable.Materials;

            var codeCell = Cell.Factory.Create(_cellPrefab, code, false, _codes, cellClickHandler);
            var typeCell = Cell.Factory.Create(_cellPrefab, type, true, _types, cellClickHandler);

            var materials = materialPanels.Select(p => p.Name.StringValue).ToList();
            var selected = materialPanels.IndexOf(materialPanels.First(p => p.Code.IntValue == coreMaterial.Code)) + 1;

            var coreMaterialCell = ReferenceCell.Factory.Create(_referenceCellPrefab, materials, selected, _coreMaterials);
            var coreDiameterCell = Cell.Factory.Create(_cellPrefab, coreDiameter, _coreDiameters, cellClickHandler);

            selected = materialPanels.IndexOf(materialPanels.First(p => p.Code.IntValue == screen1Material.Code)) + 1;

            var screen1MaterialCell = ReferenceCell.Factory.Create(_referenceCellPrefab, materials, selected, _screen1Materials);
            var screen1InnerRadiusCell = Cell.Factory.Create(_cellPrefab, screen1InnerRadius, _screen1InnerRadii, cellClickHandler);
            var screen1ThresoldCell = Cell.Factory.Create(_cellPrefab, screen1Thresold, _screen1Thresolds, cellClickHandler);
            var screen1IsolationMaterialCell = Cell.Factory.Create(_cellPrefab, screen1isolationMaterial, true, _screen1isolationMaterials, cellClickHandler);

            var nullableMaterials = Enumerable.Repeat("-", 1).Concat(materials).ToList();
            selected = screen2Material == null ? 0 : materialPanels.IndexOf(materialPanels.First(p => p.Code.IntValue == screen2Material.Code)) + 1;

            var screen2MaterialCell = ReferenceCell.Factory.Create(_referenceCellPrefab, nullableMaterials, selected, _screen2Materials);
            var screen2InnerRadiusCell = Cell.Factory.Create(_cellPrefab, screen2InnerRadius, _screen2InnerRadii, cellClickHandler);
            var screen2ThresoldCell = Cell.Factory.Create(_cellPrefab, screen2Thresold, _screen2Thresolds, cellClickHandler);
            var screen2IsolationMaterialCell = Cell.Factory.Create(_cellPrefab, screen2isolationMaterial, true, _screen2isolationMaterials, cellClickHandler);

            var crossSectionDiameterCell = Cell.Factory.Create(_cellPrefab, crossSectionDiameter, _crossSectionDiameters, cellClickHandler);

            var panel = new WireMarkPanel(codeCell, typeCell, coreMaterialCell, coreDiameterCell, screen1MaterialCell, screen1InnerRadiusCell, screen1ThresoldCell, screen1IsolationMaterialCell, screen2MaterialCell, screen2InnerRadiusCell, screen2ThresoldCell, screen2IsolationMaterialCell, crossSectionDiameterCell);
            _wireMarksPanels.Add(panel);
        }

        public override void Clear()
        {
            foreach (var panel in _wireMarksPanels)
                panel.Destroy();
        }
    }
}