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
        public abstract void LoadData(Cell cellPrefab, UnityAction<Cell> cellClickHandler);
	}
}