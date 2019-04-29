using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Management.Referencing;
using System.Linq;

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
            Cell.Factory.Create(cellPrefab, Cell.Type.Int, _coreMaterials, coreMaterial, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Float, _coreDiameters, coreDiameter, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Int, _screen1Materials, screen1Material, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Float, _screen1InnerRadii, screen1InnerRadius, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Float, _screen1Thresolds, screen1Thresold, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.String, _screen1isolationMaterials, screen1IsolationMaterial, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableInt, _screen2Materials, screen2Material, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _screen2InnerRadii, screen2InnerRadius, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _screen2Thresolds, screen2Thresold, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableString, _screen2isolationMaterials, screen2IsolationMaterial, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.Float, _crossSectionDiameters, crossSectionDiameter, cellClickHandler);
        }
    }
}