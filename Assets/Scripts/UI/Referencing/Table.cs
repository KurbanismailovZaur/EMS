using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;

namespace UI.Referencing
{
	public abstract class Table : MonoBehaviour 
	{
        public abstract void LoadData(Cell cellPrefab, Action<Cell> cellClickHandler);

        protected void ClearData()
        {
            foreach (Transform column in transform)
                foreach (Transform cell in column)
                    Destroy(cell.gameObject);
        }

        public abstract void Add(Cell cellPrefab, Action<Cell> cellClickHandler);

        public abstract (string titleRemoveName, string labelName, (string label, Action deleteHandler)[] panelsData) GetRemoveData();
    }
}