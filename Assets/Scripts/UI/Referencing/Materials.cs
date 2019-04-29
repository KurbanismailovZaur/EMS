
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

        public RectTransform Codes => _codes;

        public override void LoadData(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            ClearData();

            foreach (var material in ReferenceManager.Instance.Materials)
            {
                Add(cellPrefab, material.Code.ToString(), material.Name.ToString(), material?.Conductivity.ToString(), 
                    material?.MagneticPermeability.ToString(),  material?.DielectricConstant.ToString(), cellClickHandler);
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
    }
}