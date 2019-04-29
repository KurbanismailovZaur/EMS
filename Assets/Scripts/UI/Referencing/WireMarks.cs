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

namespace UI.Referencing
{
    public class WireMarks : Table
    {
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

        [SerializeField]
        private Materials _materials;

        public RectTransform CoreMaterials => _coreMaterials;

        public RectTransform Screen1Materials => _screen1Materials;

        public RectTransform Screen2Materials => _screen1Materials;

        public override void LoadData(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            ClearData();

            foreach (var mark in ReferenceManager.Instance.WireMarks)
                Add(cellPrefab, mark.Code.ToString(), mark.Type, mark.CoreMaterial.Code.ToString(), mark.CoreDiameter.ToString(),
                    mark.Screen1.Material.Code.ToString(), mark.Screen1.InnerRadius.ToString(), mark.Screen1.Thresold.ToString(), mark.Screen1.IsolationMaterial,
                    mark.Screen2.Material?.Code.ToString(), mark.Screen2.InnerRadius?.ToString(), mark.Screen2.Thresold?.ToString(), mark.Screen2.IsolationMaterial,
                    mark.CrossSectionDiameter.ToString(), cellClickHandler);
        }

        public override void Add(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            if (_materials.transform.childCount == 0) return;

            var code = $"м{_codes.GetChildren().Select(ch => ch.GetComponent<Cell>().Text).Max(t => int.Parse(t.text.Substring(1))) + 1}";
            var coreMaterial = _materials.Codes.GetComponentInChildren<Cell>().Text.text;
            var screen1Material = _materials.Codes.GetComponentInChildren<Cell>().Text.text;
            var screen2Material = _materials.Codes.GetComponentInChildren<Cell>().Text.text;

            Add(cellPrefab, code, null, coreMaterial, "0", screen1Material, "0", "0", "полиэтилен", null, null, null, null, "0", cellClickHandler);
        }

        private void Add(Cell cellPrefab, string code, string type, string coreMaterial, string coreDiameter,
            string screen1Material, string screen1InnerRadius, string screen1Thresold, string screen1IsolationMaterial,
            string screen2Material, string screen2InnerRadius, string screen2Thresold, string screen2IsolationMaterial,
            string crossSectionDiameter, Action<Cell> cellClickHandler)
        {
            Cell.Factory.Create(cellPrefab, Cell.Type.String, _codes, code, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.String, _types, type, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Material, _coreMaterials, coreMaterial, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Float, _coreDiameters, coreDiameter, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Material, _screen1Materials, screen1Material, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Float, _screen1InnerRadii, screen1InnerRadius, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Float, _screen1Thresolds, screen1Thresold, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.String, _screen1isolationMaterials, screen1IsolationMaterial, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableMaterial, _screen2Materials, screen2Material, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _screen2InnerRadii, screen2InnerRadius, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _screen2Thresolds, screen2Thresold, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableString, _screen2isolationMaterials, screen2IsolationMaterial, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Float, _crossSectionDiameters, crossSectionDiameter, cellClickHandler);
        }

        public List<WireMark> GetWireMarks(List<Material> materials)
        {
            var wireMarks = new List<WireMark>();

            for (int row = 0; row < _codes.childCount; row++)
            {
                WireMark wireMark = new WireMark()
                {
                    Screen1 = new WireMark.Screen(),
                    Screen2 = new WireMark.Screen()
                };

                wireMark.Code = _codes.GetChild(row).GetComponent<Cell>().Text.text;
                wireMark.Type = _types.GetChild(row).GetComponent<Cell>().Text.text;
                wireMark.CoreMaterial = materials.Find(m => m.Code == int.Parse(_coreMaterials.GetChild(row).GetComponent<Cell>().Text.text));
                wireMark.CoreDiameter = float.Parse(_coreDiameters.GetChild(row).GetComponent<Cell>().Text.text);

                wireMark.Screen1.Material = materials.Find(m => m.Code == int.Parse(_screen1Materials.GetChild(row).GetComponent<Cell>().Text.text));
                wireMark.Screen1.InnerRadius = float.Parse(_screen1InnerRadii.GetChild(row).GetComponent<Cell>().Text.text);
                wireMark.Screen1.Thresold = float.Parse(_screen1Thresolds.GetChild(row).GetComponent<Cell>().Text.text);
                wireMark.Screen1.IsolationMaterial = _screen1isolationMaterials.GetChild(row).GetComponent<Cell>().Text.text;

                var screen2Material = _screen2Materials.GetChild(row).GetComponent<Cell>().Text.text;
                wireMark.Screen2.Material = string.IsNullOrWhiteSpace(screen2Material) ? null : materials.Find(m => m.Code == int.Parse(screen2Material));

                var screen2InnerRadius = _screen2InnerRadii.GetChild(row).GetComponent<Cell>().Text.text;
                var screen2Thresolds = _screen2Thresolds.GetChild(row).GetComponent<Cell>().Text.text;

                wireMark.Screen2.InnerRadius = string.IsNullOrWhiteSpace(screen2InnerRadius) ? null : (float?)float.Parse(screen2InnerRadius);
                wireMark.Screen2.Thresold = string.IsNullOrWhiteSpace(screen2Thresolds) ? null : (float?)float.Parse(screen2Thresolds);

                wireMark.Screen2.IsolationMaterial = _screen2isolationMaterials.GetChild(row).GetComponent<Cell>().Text.text;

                wireMark.CrossSectionDiameter = float.Parse(_crossSectionDiameters.GetChild(row).GetComponent<Cell>().Text.text);

                wireMarks.Add(wireMark);
            }

            return wireMarks;
        }

        public override (string titleRemoveName, string labelName, (string label, Action deleteHandler)[] panelsData) GetRemoveData()
        {
            var panelsData = _codes.GetChildren().Select<Transform, (string, Action)>(ch => (ch.GetComponent<Cell>().Text.text, () => RemoveWireMark(ch.GetSiblingIndex()))).ToArray();

            return ("Марок Проводов", "Код", panelsData);
        }

        private void RemoveWireMark(int index)
        {
            Destroy(_codes.GetChild(index).gameObject);
            Destroy(_types.GetChild(index).gameObject);
            Destroy(_coreMaterials.GetChild(index).gameObject);
            Destroy(_coreDiameters.GetChild(index).gameObject);
            Destroy(_screen1Materials.GetChild(index).gameObject);
            Destroy(_screen1InnerRadii.GetChild(index).gameObject);
            Destroy(_screen1Thresolds.GetChild(index).gameObject);
            Destroy(_screen1isolationMaterials.GetChild(index).gameObject);
            Destroy(_screen2Materials.GetChild(index).gameObject);
            Destroy(_screen2InnerRadii.GetChild(index).gameObject);
            Destroy(_screen2Thresolds.GetChild(index).gameObject);
            Destroy(_screen2isolationMaterials.GetChild(index).gameObject);
            Destroy(_crossSectionDiameters.GetChild(index).gameObject);
        }
    }
}