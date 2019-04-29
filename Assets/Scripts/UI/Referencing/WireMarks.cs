using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using Management.Referencing;

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

        [SerializeField]
        private RectTransform _crossSectionDiameters;
        #endregion

        public override void LoadData(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            ClearData();

            foreach (var mark in ReferenceManager.Instance.WireMarks)
            {
                Cell.Factory.Create(cellPrefab, Cell.Type.String, _codes, mark.Code.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.String, _types, mark.Type, cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.Material, _coreMaterials, mark.CoreMaterial.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.Float, _coreDiameters, mark.CoreDiameter.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.Material, _screen1Materials, mark.Screen1.Material.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.Float, _screen1InnerRadii, mark.Screen1.InnerRadius.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.Float, _screen1Thresolds, mark.Screen1.Thresold.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.String, _screen1isolationMaterials, mark.Screen1.IsolationMaterial, cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.NullableMaterial, _screen2Materials, mark.Screen2.Material?.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _screen2InnerRadii, mark.Screen2.InnerRadius?.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _screen2Thresolds, mark.Screen2.Thresold?.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.NullableString, _screen2isolationMaterials, mark.Screen2.IsolationMaterial, cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.Float, _crossSectionDiameters, mark.CrossSectionDiameter.ToString(), cellClickHandler);
            }
        }
    }
}