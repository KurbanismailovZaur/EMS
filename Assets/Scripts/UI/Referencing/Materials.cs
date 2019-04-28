
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

        public override void LoadData(Cell cellPrefab, UnityAction<Cell> cellClickHandler)
        {
            ClearData();

            foreach (var material in ReferenceManager.Instance.Materials)
            {
                InstantiateCell(cellPrefab, _codes, material.Code.ToString(), cellClickHandler);
                InstantiateCell(cellPrefab, _names, material.Name.ToString(), cellClickHandler);
                InstantiateCell(cellPrefab, _conductivities, material.Conductivity.ToString(), cellClickHandler);
                InstantiateCell(cellPrefab, _magneticPermeabilities, material.MagneticPermeability.ToString(), cellClickHandler);
                InstantiateCell(cellPrefab, _dielectricConstants, material.DielectricConstant.ToString(), cellClickHandler);
            }
        }

        private void ClearData()
        {
            foreach (Transform column in transform)
                foreach (Transform cell in column)
                    Destroy(cell.gameObject);
        }

        private Cell InstantiateCell(Cell cellPrefab, Transform parent, string value, UnityAction<Cell> cellClickHandler)
        {
            var cell = Instantiate(cellPrefab, parent);
            cell.Text.text = value;

            cell.Clicked.AddListener(cellClickHandler);

            return cell;
        }
    }
}