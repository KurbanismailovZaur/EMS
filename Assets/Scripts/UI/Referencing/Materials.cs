
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using UnityEngine.UI;
using Management.Referencing;
using System;
using UnityEngine.Events;

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

        public override void LoadData(Cell cellPrefab, Action<Cell> cellClickHandler)
        {
            ClearData();

            foreach (var material in ReferenceManager.Instance.Materials)
            {
                Cell.Factory.Create(cellPrefab, Cell.Type.Int, _codes, material.Code.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.String, _names, material.Name.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _conductivities, material?.Conductivity.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _magneticPermeabilities, material?.MagneticPermeability.ToString(), cellClickHandler);
                Cell.Factory.Create(cellPrefab, Cell.Type.NullableFloat, _dielectricConstants, material?.DielectricConstant.ToString(), cellClickHandler);
            }
        }
    }
}