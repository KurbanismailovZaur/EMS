using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;
using UnityEngine.Events;

namespace UI.Referencing.Tables
{
	public abstract class Table : MonoBehaviour 
	{
        [SerializeField]
        protected Cell _cellPrefab;

        [SerializeField]
        protected ReferenceCell _referenceCellPrefab;

        public abstract void Clear();

        public abstract void AddEmpty(Action<Cell> cellClickHandler);
    }
}