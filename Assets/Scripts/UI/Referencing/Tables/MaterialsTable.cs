
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using Management.Referencing;
using System;
using UnityEngine.Events;
using System.Linq;
using Material = Management.Referencing.Material;
using UnityObject = UnityEngine.Object;
using System.Collections.ObjectModel;

namespace UI.Referencing.Tables
{
    public class MaterialsTable : Table
    {
        public class MaterialPanel
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
            }

            public void Destroy()
            {
                UnityObject.Destroy(Code.gameObject);
                UnityObject.Destroy(Name.gameObject);
                UnityObject.Destroy(Conductivity.gameObject);
                UnityObject.Destroy(MagneticPermeability.gameObject);
                UnityObject.Destroy(DielectricConstant.gameObject);
            }
        }

        [SerializeField]
        private RectTransform _codes;

        [SerializeField]
        private RectTransform _names;

        [SerializeField]
        private RectTransform _conductivities;

        [SerializeField]
        private RectTransform _magneticPermeabilities;

        [SerializeField]
        private RectTransform _dielectricConstants;

        private List<MaterialPanel> _materialPanels = new List<MaterialPanel>();

        public ReadOnlyCollection<MaterialPanel> Materials => new ReadOnlyCollection<MaterialPanel>(_materialPanels);

        public void AddMaterials(Action<Cell> cellClickHandler)
        {
            foreach (var material in ReferenceManager.Instance.Materials)
                Add(material.Code, material.Name, material.Conductivity, material.MagneticPermeability, material.DielectricConstant, cellClickHandler);
        }

        private void Add(int code, string name, float? conductivity, float? magneticPermeability, float? dielectricConstant, Action<Cell> cellClickHandler)
        {
            var codeCell = Cell.Factory.Create(_cellPrefab, code, _codes, cellClickHandler);
            var nameCell = Cell.Factory.Create(_cellPrefab, name, true, _names, cellClickHandler);
            var conductivityCell = Cell.Factory.Create(_cellPrefab, conductivity, _conductivities, cellClickHandler);
            var magneticPermeabilityCell = Cell.Factory.Create(_cellPrefab, magneticPermeability, _magneticPermeabilities, cellClickHandler);
            var dielectricConstantCell = Cell.Factory.Create(_cellPrefab, dielectricConstant, _dielectricConstants, cellClickHandler);

            var panel = new MaterialPanel(codeCell, nameCell, conductivityCell, magneticPermeabilityCell, dielectricConstantCell);
            _materialPanels.Add(panel);
        }

        public override void AddEmpty(Action<Cell> cellClickHandler)
        {
            Add(GetNextCode(), "Материал", null, null, null, cellClickHandler);
        }

        private int GetNextCode() => _materialPanels.Max(p => p.Code).IntValue + 1;

        public override void Clear()
        {
            foreach (var panel in _materialPanels)
                panel.Destroy();
        }
    }
}