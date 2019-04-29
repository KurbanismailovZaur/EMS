
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

namespace UI.Referencing
{
    public class Materials : Table
    {
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

        [SerializeField]
        private WireMarks _wireMarks;

        public RectTransform Codes => _codes;

        public override void LoadData(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            ClearData();

            foreach (var material in ReferenceManager.Instance.Materials)
            {
                Add(cellPrefab, material.Code.ToString(), material.Name.ToString(), material?.Conductivity.ToString(),
                    material?.MagneticPermeability.ToString(), material?.DielectricConstant.ToString(), cellClickHandler);
            }
        }

        public override void Add(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            var code = _codes.GetChildren().Select(ch => ch.GetComponent<Cell>().Text).Max(t => int.Parse(t.text)) + 1;

            Add(cellPrefab, code.ToString(), "материал", null, null, null, cellClickHandler);
        }

        private void Add(Cell cellPrefab, string code, string name, string conductivity, string magneticPermeability, string dielectricConstant, Action<Cell> cellClickHandler)
        {
            Cell.Factory.Create(cellPrefab, Cell.Type.Int, _codes, code, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.String, _names, name, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _conductivities, conductivity, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _magneticPermeabilities, magneticPermeability, cellClickHandler);
            Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _dielectricConstants, dielectricConstant, cellClickHandler);
        }

        public List<Material> GetMaterials()
        {
            var materials = new List<Material>();

            for (int row = 0; row < _codes.childCount; row++)
            {
                Material material = new Material();
                material.Code = int.Parse(_codes.GetChild(row).GetComponent<Cell>().Text.text);
                material.Name = _names.GetChild(row).GetComponent<Cell>().Text.text;

                var conductivity = _conductivities.GetChild(row).GetComponent<Cell>().Text.text;
                var magneticPermeability = _conductivities.GetChild(row).GetComponent<Cell>().Text.text;
                var dielectricConstant = _conductivities.GetChild(row).GetComponent<Cell>().Text.text;

                material.Conductivity = string.IsNullOrWhiteSpace(conductivity) ? null : (float?)float.Parse(conductivity);
                material.MagneticPermeability = string.IsNullOrWhiteSpace(magneticPermeability) ? null : (float?)float.Parse(magneticPermeability);
                material.DielectricConstant = string.IsNullOrWhiteSpace(dielectricConstant) ? null : (float?)float.Parse(dielectricConstant);

                materials.Add(material);
            }

            return materials;
        }

        public override (string titleRemoveName, string labelName, (string label, Action deleteHandler)[] panelsData) GetRemoveData()
        {
            var materialsCodes = _wireMarks.CoreMaterials.GetChildren().Concat(_wireMarks.Screen1Materials.GetChildren()).Concat(_wireMarks.Screen2Materials.GetChildren()).Select(ch => ch.GetComponent<Cell>().Text.text).Distinct();
            var panelsData = _codes.GetChildren().Where(ch => materialsCodes.All(c => ch.GetComponent<Cell>().Text.text != c)).Select<Transform, (string, Action)>(ch => (_names.GetChild(ch.GetSiblingIndex()).GetComponent<Cell>().Text.text, () => RemoveMaterial(ch.GetSiblingIndex()))).ToArray();

            return ("Материалов", "Название", panelsData);
        }

        private void RemoveMaterial(int index)
        {
            Destroy(_codes.GetChild(index).gameObject);
            Destroy(_names.GetChild(index).gameObject);
            Destroy(_conductivities.GetChild(index).gameObject);
            Destroy(_magneticPermeabilities.GetChild(index).gameObject);
            Destroy(_dielectricConstants.GetChild(index).gameObject);
        }
    }
}